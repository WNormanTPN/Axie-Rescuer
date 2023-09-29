using AxieRescuer;
using Unity.Entities;
using UnityEditor.Search;
namespace AxieRescuer
{
    public partial struct TakeItemSystem : ISystem
    {
        public EntityQuery PlayerQuery;
        public void OnCreate(ref SystemState state)
        {
            PlayerQuery = SystemAPI.QueryBuilder()
                .WithAll<DroppedItem>().Build();
            state.RequireForUpdate(PlayerQuery);
        }
        public void OnUpdate(ref SystemState state)
        {
            var player = SystemAPI.GetSingletonEntity<PlayerTag>();
        }
    }
}