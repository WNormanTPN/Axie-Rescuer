using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AxieRescuer
{
    public struct SpawnBuffer : IBufferElementData
    { 
        public Entity Entity;
        public LocalTransform Transform;
    }
}
