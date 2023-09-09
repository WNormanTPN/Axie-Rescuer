using Unity.AI.Navigation;
using Unity.Entities;

namespace ProjectDawn.Navigation.Hybrid
{
    public class NavMeshSurfaceBaker : Baker<NavMeshSurface>
    {
        public override void Bake(NavMeshSurface authoring)
        {
#if UNITY_ENTITIES_VERSION_65
            AddSharedComponentManaged(GetEntity(TransformUsageFlags.Dynamic), new NavMeshData { Value = authoring.navMeshData });
#else
            AddSharedComponentManaged(new NavMeshData { Value = authoring.navMeshData });
#endif
        }
    }
}
