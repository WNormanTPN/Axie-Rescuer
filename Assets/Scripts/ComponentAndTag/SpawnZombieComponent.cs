using Unity.Entities;
using Unity.Mathematics;

namespace AxieRescuer
{
    public struct SpawnZombieComponent : IComponentData
    {
        public Entity Entity;
        public float Value;
        public float2 StartMap;
        public float2 EndMap;
    }
}