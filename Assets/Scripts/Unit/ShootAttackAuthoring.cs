using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Unit
{
    public class ShootAttackAuthoring : MonoBehaviour
    {
        public float timerMax;
        public int damageAmount;
        public float attackDistance;
        public Transform bulletSpawnLocalPosition;

        private class ShootAttackBaker : Baker<ShootAttackAuthoring>
        {
            public override void Bake(ShootAttackAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ShootAttack
                {
                    timerMax = authoring.timerMax,
                    timer = 0,
                    damageAmount = authoring.damageAmount,
                    attackDistance = authoring.attackDistance,
                    bulletSpawnLocalPosition = authoring.bulletSpawnLocalPosition.localPosition,
                });
            }
        }
    }

    public struct ShootAttack : IComponentData
    {
        public float timer;
        public float timerMax;
        public int damageAmount;
        public float attackDistance;
        public float3 bulletSpawnLocalPosition;
    }
}