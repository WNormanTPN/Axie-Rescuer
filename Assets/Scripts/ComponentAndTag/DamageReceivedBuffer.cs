using Unity.Entities;

namespace AxieRescuer
{
    public struct ZombieAttackTimer : IComponentData
    {
        public float Value;
    }

    public struct DamageReceived : IBufferElementData
    {
        public float Value;
    }
}