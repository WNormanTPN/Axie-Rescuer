using Unity.Entities;

namespace ProjectDawn.Navigation
{
    [UpdateAfter(typeof(AgentForceSystemGroup))]
    [UpdateInGroup(typeof(AgentSystemGroup))]
    public partial class AgentTransformSystemGroup : ComponentSystemGroup { }
}
