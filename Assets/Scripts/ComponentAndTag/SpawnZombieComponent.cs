using Unity.Entities;

namespace AxieRescuer
{
    public struct SpawnZombieComponent : IComponentData
    {
        public Entity Entity;
        public float Range;
    }
}