using Unity.Entities;
using Unity.VisualScripting;

namespace AxieRescuer
{
    public struct ZombiePositionBuffer : IBufferElementData
    {
        public Entity Entity;
    }
}