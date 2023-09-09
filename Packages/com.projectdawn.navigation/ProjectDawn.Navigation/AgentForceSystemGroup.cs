using Unity.Entities;

namespace ProjectDawn.Navigation
{
    [UpdateAfter(typeof(AgentSteeringSystemGroup))]
    [UpdateInGroup(typeof(AgentSystemGroup))]
    public partial class AgentForceSystemGroup : ComponentSystemGroup { }
}
