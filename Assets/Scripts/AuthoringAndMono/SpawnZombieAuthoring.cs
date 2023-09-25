using Unity.Entities;
using UnityEngine;

namespace AxieRescuer
{
    public class SpawnZombieAuthoring : MonoBehaviour
    {
        public GameObject ZombiePrefabs;
        public float Range;
        public class SpawnZombieBaking : Baker<SpawnZombieAuthoring>
        {
            public override void Bake(SpawnZombieAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<SpawnEnemyTag>(entity);
                AddComponent(entity, new SpawnZombieComponent
                {
                    Entity = GetEntity(authoring.ZombiePrefabs,TransformUsageFlags.Dynamic),
                    Range = authoring.Range,
                });
            }
        }
    }
}