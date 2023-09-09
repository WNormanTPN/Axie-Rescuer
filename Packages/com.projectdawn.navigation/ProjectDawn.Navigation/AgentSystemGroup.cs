using Unity.Entities;

namespace ProjectDawn.Navigation
{
#if AGENTS_NAVIGATION_REGULAR_UPDATE
    [UpdateInGroup(typeof(SimulationSystemGroup))]
#else
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
#endif
    public partial class AgentSystemGroup : ComponentSystemGroup { }
}
