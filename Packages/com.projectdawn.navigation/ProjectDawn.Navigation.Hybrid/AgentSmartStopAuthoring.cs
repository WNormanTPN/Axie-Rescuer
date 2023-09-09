using Unity.Entities;
using UnityEngine;

namespace ProjectDawn.Navigation.Hybrid
{
    /// <summary>
    /// Enables collisio with other agents.
    /// </summary>
    [RequireComponent(typeof(AgentAuthoring))]
    [AddComponentMenu("Agents Navigation/Agent Smart Stop")]
    [DisallowMultipleComponent]
    [HelpURL("https://lukaschod.github.io/agents-navigation-docs/manual/game-objects/smart-stop.html")]
    public class AgentSmartStopAuthoring : MonoBehaviour
    {
        /// <summary>
        /// This option allows agent to do smarter stop decision than moving in group.
        /// It works under assumption that by reaching nearby agent that is already idle and have similar destination it can stop as destination is reached.
        /// </summary>
        [SerializeField]
        protected HiveMindStop m_HiveMindStop = HiveMindStop.Default;

        Entity m_Entity;

        /// <summary>
        /// Returns default component of <see cref="AgentSeparation"/>.
        /// </summary>
        public AgentSmartStop DefaulSmartStop => new AgentSmartStop
        {
            HiveMindStop = m_HiveMindStop,
        };

        /// <summary>
        /// <see cref="AgentSmartStop"/> component of this <see cref="AgentAuthoring"/> Entity.
        /// Accessing this property is potentially heavy operation as it will require wait for agent jobs to finish.
        /// </summary>
        public AgentSmartStop EntitySmartStop
        {
            get => World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<AgentSmartStop>(m_Entity);
            set => World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(m_Entity, value);
        }

        /// <summary>
        /// Returns true if <see cref="AgentAuthoring"/> entity has <see cref="AgentSmartStop"/>.
        /// </summary>
        public bool HasEntitySmartStop => World.DefaultGameObjectInjectionWorld != null && World.DefaultGameObjectInjectionWorld.EntityManager.HasComponent<AgentSmartStop>(m_Entity);

        void Awake()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            m_Entity = GetComponent<AgentAuthoring>().GetOrCreateEntity();
            world.EntityManager.AddComponentData(m_Entity, DefaulSmartStop);
        }

        void OnDestroy()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world != null)
                world.EntityManager.RemoveComponent<AgentSmartStop>(m_Entity);
        }

        void OnEnable()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null)
                return;
            world.EntityManager.SetComponentEnabled<AgentSmartStop>(m_Entity, true);
        }

        void OnDisable()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null)
                return;
            world.EntityManager.SetComponentEnabled<AgentSmartStop>(m_Entity, false);
        }
    }

    internal class AgentSmartStopBaker : Baker<AgentSmartStopAuthoring>
    {
        public override void Bake(AgentSmartStopAuthoring authoring)
        {
#if UNITY_ENTITIES_VERSION_65
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), authoring.DefaulSmartStop);
#else
            AddComponent(authoring.DefaulSmartStop);
#endif
        }
    }
}
