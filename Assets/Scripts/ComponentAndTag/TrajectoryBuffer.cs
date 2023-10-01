using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AxieRescuer
{
    public struct Trajectory : IBufferElementData
    {
        public float3 Start;
        public float3 End;
        public float Width;
        public float ShowTime;
    }
}