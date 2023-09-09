using Unity.Entities;
using Unity.Mathematics;

namespace AxieRescuer
{
    public struct FollowOffset : IComponentData
    {
        public float3 Position;
        public float3 Rotation;
    }
}