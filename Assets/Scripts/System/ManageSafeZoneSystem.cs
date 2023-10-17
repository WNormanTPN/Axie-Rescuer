using Unity.Burst;
using Unity.Entities;

namespace AxieRescuer
{
    public partial struct ManageSafeZoneSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var axieQuery = SystemAPI.QueryBuilder()
                .Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

        }
    }
}