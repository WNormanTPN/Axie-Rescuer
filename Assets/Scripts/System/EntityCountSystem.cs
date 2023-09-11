using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace AxieRescuer
{
    public partial struct EntityCountSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            int i = 0;
            foreach(var s in SystemAPI.Query<Simulate>())
            {
                i++;
            }
            Debug.Log(i);
        }
    }
}