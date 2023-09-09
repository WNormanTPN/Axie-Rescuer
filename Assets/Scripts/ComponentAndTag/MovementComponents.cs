using Unity.Entities;
using Unity.Mathematics;

namespace AxieRescuer
{
    public struct MoveDirection : IComponentData
    {
        public float3 Value;
    }

    public struct MoveSpeed : IComponentData
    {
        public float Value;
    }

    public struct TargetRotationDirection : IComponentData
    {
        public float3 Value;
    }

    public struct RotationSpeed : IComponentData
    {
        public float Value;
    }
}