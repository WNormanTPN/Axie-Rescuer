using Unity.Entities;

namespace ProjectDawn.Navigation
{
    public struct AgentReciprocalAvoid : IComponentData
    {
        public float Radius;

        public static AgentReciprocalAvoid Default => new()
        {
            Radius = 2
        };
    }
}
