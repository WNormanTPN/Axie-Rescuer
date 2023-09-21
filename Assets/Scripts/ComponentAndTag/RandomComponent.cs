using Unity.Entities;
using Unity.Mathematics;

namespace AxieRescuer
{
    public struct RandomSingleton : IComponentData
    {
        public Random Random;
    }
}