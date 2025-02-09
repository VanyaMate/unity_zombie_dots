using System;
using Assets;
using Movement;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Interact
{
    public class InteractWorld : MonoBehaviour
    {
        public static InteractWorld Instance { get; set; }

        public event EventHandler OnSelectionAreaStart;
        public event EventHandler OnSelectionAreaEnd;

        private Vector2 _selectionStartMousePosition = Vector2.zero;

        public void Awake()
        {
            Instance = this;
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                this._selectionStartMousePosition = Input.mousePosition;
                this.OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector2 selectionEndMousePosition = Input.mousePosition;
                this.OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);

                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                // Unselect all
                EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<Selected>()
                    .Build(entityManager);
                NativeArray<Entity> entities = entityQuery.ToEntityArray(Allocator.Temp);
                NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);
                for (int i = 0; i < entities.Length; i++)
                {
                    entityManager.SetComponentEnabled<Selected>(entities[i], false);
                    Selected selected = selectedArray[i];
                    selected.onDeselected = true;
                    entityManager.SetComponentData(entities[i], selected);
                }
                // 

                entityQuery =
                    new EntityQueryBuilder(Allocator.Temp)
                        .WithAll<LocalTransform, Unit.Unit>()
                        .WithPresent<Selected>()
                        .Build(entityManager);

                entities = entityQuery.ToEntityArray(Allocator.Temp);
                NativeArray<LocalTransform> localTransforms =
                    entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
                Rect selectionArea = GetSelectionAreaRect();
                float selectionAreaSize = selectionArea.width + selectionArea.height;
                float multipleSelectionSizeMin = 40f;
                bool isMultipleSelection = selectionAreaSize > multipleSelectionSizeMin;

                if (isMultipleSelection)
                {
                    for (int i = 0; i < localTransforms.Length; i++)
                    {
                        LocalTransform localTransform = localTransforms[i];
                        Vector2 unitScreenPosition =
                            Camera.main.WorldToScreenPoint(localTransform.Position);
                        Vector2 unitScreenPositionUp =
                            Camera.main.WorldToScreenPoint(localTransform.Position + math.up() * 1.5f);
                        if (
                            selectionArea.Overlaps(new Rect
                            {
                                x = unitScreenPosition.x,
                                y = unitScreenPosition.y,
                                width = unitScreenPositionUp.x - unitScreenPosition.x,
                                height = unitScreenPositionUp.y - unitScreenPosition.y
                            })
                        )
                        {
                            entityManager.SetComponentEnabled<Selected>(entities[i], true);
                            Selected selected = entityManager.GetComponentData<Selected>(entities[i]);
                            selected.onSelected = true;
                            entityManager.SetComponentData(entities[i], selected);
                        }
                    }
                }
                else
                {
                    entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                    PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                    UnityEngine.Ray pointOfRaycast = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (
                        physicsWorldSingleton.CollisionWorld.CastRay(
                            new RaycastInput()
                            {
                                Start = pointOfRaycast.GetPoint(0f),
                                End = pointOfRaycast.GetPoint(100f),
                                Filter = new CollisionFilter()
                                {
                                    CollidesWith = 1u << GameAssets.UNITS_LAYER,
                                    BelongsTo = ~0u,
                                    GroupIndex = 0
                                }
                            },
                            out RaycastHit raycastHit
                        )
                    )
                    {
                        if (entityManager.HasComponent<Unit.Unit>(raycastHit.Entity) &&
                            entityManager.HasComponent<Selected>(raycastHit.Entity))
                        {
                            entityManager.SetComponentEnabled<Selected>(raycastHit.Entity, true);
                            Selected selected = entityManager.GetComponentData<Selected>(raycastHit.Entity);
                            selected.onSelected = true;
                            entityManager.SetComponentData(raycastHit.Entity, selected);
                        }
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                Vector3 position = Mouse.World.GetPosition();
                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                EntityQuery entityQuery =
                    new EntityQueryBuilder(Allocator.Temp)
                        .WithAll<UnitMovementComponent, Selected>()
                        .Build(entityManager);

                NativeArray<Entity> entities = entityQuery.ToEntityArray(Allocator.Temp);
                NativeArray<UnitMovementComponent> unitMovementComponents =
                    entityQuery.ToComponentDataArray<UnitMovementComponent>(Allocator.Temp);
                NativeArray<float3> positionArray = this.GenerateMovePositionArray(position, entities.Length);

                for (int i = 0; i < unitMovementComponents.Length; i++)
                {
                    UnitMovementComponent componentData = unitMovementComponents[i];
                    componentData.targetPosition = positionArray[i];
                    entityManager.SetComponentData(entities[i], componentData);
                }
            }
        }

        public Rect GetSelectionAreaRect()
        {
            Vector2 currentMousePosition = Input.mousePosition;
            Vector2 lowerLeftCorner = new Vector2(
                Mathf.Min(this._selectionStartMousePosition.x, currentMousePosition.x),
                Mathf.Min(this._selectionStartMousePosition.y, currentMousePosition.y)
            );
            Vector2 upperRightCorner = new Vector2(
                Mathf.Max(this._selectionStartMousePosition.x, currentMousePosition.x),
                Mathf.Max(this._selectionStartMousePosition.y, currentMousePosition.y)
            );

            return new Rect(
                lowerLeftCorner.x,
                lowerLeftCorner.y,
                upperRightCorner.x - lowerLeftCorner.x,
                upperRightCorner.y - lowerLeftCorner.y
            );
        }

        private NativeArray<float3> GenerateMovePositionArray(float3 targetPosition, int positionCount)
        {
            NativeArray<float3> positionArray = new NativeArray<float3>(positionCount, Allocator.Temp);
            if (positionCount == 0)
            {
                return positionArray;
            }

            positionArray[0] = targetPosition;
            if (positionCount == 1)
            {
                return positionArray;
            }

            float ringSize = 2.2f;
            int ring = 0;
            int positionIndex = 1;

            while (positionIndex < positionCount)
            {
                int ringPositionCount = 3 + ring * 2;

                for (int i = 0; i < ringPositionCount; i++)
                {
                    float angle = i * (math.PI2 / ringPositionCount);
                    float3 ringVector = math.rotate(quaternion.RotateY(angle), new float3(ringSize * (ring + 1), 0, 0));
                    float3 ringPosition = targetPosition + ringVector;
                    positionArray[positionIndex] = ringPosition;
                    positionIndex++;

                    if (positionIndex >= positionCount)
                    {
                        break;
                    }
                }

                ring++;
            }

            return positionArray;
        }
    }
}