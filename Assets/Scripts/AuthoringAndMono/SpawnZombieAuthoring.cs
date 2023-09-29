using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AxieRescuer
{
    public class SpawnZombieAuthoring : MonoBehaviour
    {
        public GameObject ZombiePrefabs;
        public float Value;
        public float2 StartMap;
        public float2 EndMap;
        public class SpawnZombieBaking : Baker<SpawnZombieAuthoring>
        {
            public override void Bake(SpawnZombieAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<SpawnEnemyTag>(entity);
                AddComponent(entity, new SpawnZombieComponent
                {
                    Entity = GetEntity(authoring.ZombiePrefabs,TransformUsageFlags.Dynamic),
                    Value = authoring.Value,
                    StartMap = authoring.StartMap,
                    EndMap = authoring.EndMap
                });
            }
        }
    }
}