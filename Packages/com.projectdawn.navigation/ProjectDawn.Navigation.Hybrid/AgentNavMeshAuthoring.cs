using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ProjectDawn.Navigation.Hybrid
{
    /// <summary>
    /// Agent uses NavMesh for pathfinding.
    /// </summary>
    [RequireComponent(typeof(AgentAuthoring))]
    [AddComponentMenu("Agents Navigation/Agent NavMesh Pathing")]
    [DisallowMultipleComponent]
    [HelpURL("https://lukaschod.github.io/agents-navigation-docs/manual/game-objects/pathing/nav-mesh.html")]
    public class AgentNavMeshAuthoring : MonoBehaviour
    {
        [SerializeField]
        protected int AgentTypeId = 0;

        [SerializeField]
        protected int AreaMask = -1;

        [SerializeField]
        protected bool AutoRepath = true;

        [SerializeField]
        protected float3 MappingExtent = 10;

        Entity m_Entity;

        /// <summary>
        /// Returns default component of <see cref="NavMeshPath"/>.
        /// </summary>
        public NavMeshPath DefaulPath => new NavMeshPath
        {
            State = NavMeshPathState.Finished,
            AgentTypeId = AgentTypeId,
            AreaMask = AreaMask,
            AutoRepath = AutoRepath,
            MappingExtent = MappingExtent,
        };

        /// <summary>
        /// <see cref="NavMeshPath"/> component of this <see cref="AgentAuthoring"/> Entity.
        /// Accessing this property is potentially heavy operation as it will require wait for agent jobs to finish.
        /// </summary>
        public NavMeshPath EntityPath
        {
            get => World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<NavMeshPath>(m_Entity);
            set => World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(m_Entity, value);
        }

        /// <summary>
        /// <see cref="NavMeshNode"/> component of this <see cref="AgentAuthoring"/> Entity.
        /// Accessing this property is potentially heavy operation as it will require wait for agent jobs to finish.
        /// </summary>
        public DynamicBuffer<NavMeshNode> EntityNodes
        {
            get => World.DefaultGameObjectInjectionWorld.EntityManager.GetBuffer<NavMeshNode>(m_Entity);
        }

        /// <summary>
        /// Returns true if <see cref="AgentAuthoring"/> entity has <see cref="NavMeshPath"/>.
        /// </summary>
        public bool HasEntityPath => World.DefaultGameObjectInjectionWorld != null && World.DefaultGameObjectInjectionWorld.EntityManager.HasComponent<NavMeshPath>(m_Entity);

        void Awake()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            m_Entity = GetComponent<AgentAuthoring>().GetOrCreateEntity();
            world.EntityManager.AddComponentData(m_Entity, DefaulPath);
            world.EntityManager.AddBuffer<NavMeshNode>(m_Entity);

            // Sync in case it was created as disabled
            if (!enabled)
                world.EntityManager.SetComponentEnabled<NavMeshPath>(m_Entity, false);
        }

        void OnDestroy()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world != null)
            {
                world.EntityManager.RemoveComponent<NavMeshPath>(m_Entity);
                world.EntityManager.RemoveComponent<NavMeshNode>(m_Entity);
            }
        }

        void OnEnable()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null)
                return;
            world.EntityManager.SetComponentEnabled<NavMeshPath>(m_Entity, true);
        }

        void OnDisable()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null)
                return;
            world.EntityManager.SetComponentEnabled<NavMeshPath>(m_Entity, false);
        }
    }

    internal class AgentNavMeshBaker : Baker<AgentNavMeshAuthoring>
    {
        public override void Bake(AgentNavMeshAuthoring authoring)
        {
#if UNITY_ENTITIES_VERSION_65
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, authoring.DefaulPath);
            AddBuffer<NavMeshNode>(entity);
#else
            AddComponent(authoring.DefaulPath);
            AddBuffer<NavMeshNode>();
#endif
        }
    }
}
