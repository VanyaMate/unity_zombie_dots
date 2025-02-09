using Unity.Entities;
using UnityEngine;

namespace Unit
{
    public class EntitiesReferencesAuthoring : MonoBehaviour
    {
        public GameObject bulletPrefab;
        public GameObject zombiePrefab;

        private class EntitiesReferencesBaker : Baker<EntitiesReferencesAuthoring>
        {
            public override void Bake(EntitiesReferencesAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EntitiesReferences
                {
                    bulletPrefabEntity = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic),
                    zombiePrefabEntity = GetEntity(authoring.zombiePrefab, TransformUsageFlags.Dynamic),
                });
            }
        }
    }

    public struct EntitiesReferences : IComponentData
    {
        public Entity bulletPrefabEntity;
        public Entity zombiePrefabEntity;
    }
}