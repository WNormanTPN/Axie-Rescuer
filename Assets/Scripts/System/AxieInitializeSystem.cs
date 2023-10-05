using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace AxieRescuer
{
    public partial struct AxieInitializeSystem : ISystem
    {
        private EntityQuery _axieQuery;
        private EntityQuery _buildingQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _axieQuery = SystemAPI.QueryBuilder()
                .WithAll<AxieNeedInitTag>()
                .WithAll<LocalTransform>()
                .WithAll<CharacterGameObjectPrefab>()
                .Build();
            state.RequireForUpdate(_axieQuery);

            _buildingQuery = SystemAPI.QueryBuilder()
                .WithAll<LocalTransform>()
                .WithAll<BuildingTag>()
                .Build();
            state.RequireForUpdate(_buildingQuery);
        }

        
        public void OnUpdate(ref SystemState state)
        {
            
        }
    }
    [BurstCompile]
    public partial struct SetAxieToBuildingJob : IJobEntity
    {
        public void Execute
        (
            
        )
        {

        }
    }
}