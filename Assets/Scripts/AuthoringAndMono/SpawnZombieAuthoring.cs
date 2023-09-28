using Unity.Entities;
using UnityEngine;

namespace AxieRescuer
{
    public class SpawnZombieAuthoring : MonoBehaviour
    {
        public GameObject ZombiePrefabs;
        public float Value;
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
                });
            }
        }
    }
}