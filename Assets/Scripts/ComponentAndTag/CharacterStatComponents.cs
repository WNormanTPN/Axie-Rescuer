using Unity.Entities;

namespace AxieRescuer
{
    public struct Health : IComponentData
    {
        public float Max;
        public float Current;
    }

    public struct Oxygen : IComponentData
    {
        public float Max;
        public float Current;
    }
}