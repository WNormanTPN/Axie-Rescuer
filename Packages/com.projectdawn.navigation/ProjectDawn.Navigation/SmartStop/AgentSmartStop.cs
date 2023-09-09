using Unity.Entities;

namespace ProjectDawn.Navigation
{
    /// <summary>
    /// This option allows agent to do smarter stop decision than moving in group.
    /// It works under assumption that by reaching nearby agent that is already idle and have similar destination it can stop as destination is reached.
    /// </summary>
    [System.Serializable]
    public struct HiveMindStop
    {
        public bool Enabled;
        /// <summary>
        /// Radius at which agent will assume similarity of the distance.
        /// </summary>
        [UnityEngine.Tooltip("Radius at which agent will assume similarity of the distance.")]
        public float Radius;

        /// <summary>
        /// Returns default configuration.
        /// </summary>
        public static HiveMindStop Default => new()
        {
            Enabled = true,
            Radius = 1,
        };
    }

    /// <summary>
    /// Agent smart stop to handle some common stuck scenarios.
    /// </summary>
    public struct AgentSmartStop : IComponentData, IEnableableComponent
    {
        /// <summary>
        /// This option allows agent to do smarter stop decision than moving in group.
        /// It works under assumption that by reaching nearby agent that is already idle and have similar destination it can stop as destination is reached.
        /// </summary>
        public HiveMindStop HiveMindStop;

        /// <summary>
        /// Returns default configuration.
        /// </summary>
        public static AgentSmartStop Default => new()
        {
            HiveMindStop = HiveMindStop.Default,
        };
    }
}
