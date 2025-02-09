﻿using Unity.Entities;
using UnityEngine;

namespace Unit
{
    public class TargetAuthoring : MonoBehaviour
    {
        public GameObject targetGameObject;

        private class TargetBaker : Baker<TargetAuthoring>
        {
            public override void Bake(TargetAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Target
                {
                    targetEntity = GetEntity(authoring.targetGameObject, TransformUsageFlags.Dynamic),
                });
            }
        }
    }

    public struct Target : IComponentData
    {
        public Entity targetEntity;
    }
}