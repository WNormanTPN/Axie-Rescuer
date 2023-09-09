using Unity.Entities;
using UnityEngine;

namespace ProjectDawn.Navigation.Hybrid
{
    /// <summary>
    /// Enables collisio with other agents.
    /// </summary>
    [RequireComponent(typeof(AgentAuthoring))]
    [AddComponentMenu("Agents Navigation/Agent Collider")]
    [DisallowMultipleComponent]
    [HelpURL("https://lukaschod.github.io/agents-navigation-docs/manual/game-objects/collider.html")]
    public class AgentColliderAuthoring : MonoBehaviour
    {
        Entity m_Entity;

        void Awake()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            m_Entity = GetComponent<AgentAuthoring>().GetOrCreateEntity();
            world.EntityManager.AddComponentData(m_Entity, new AgentCollider { });
        }

        void OnDestroy()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world != null)
                world.EntityManager.RemoveComponent<AgentCollider>(m_Entity);
        }
    }

    internal class AgentColliderBaker : Baker<AgentColliderAuthoring>
    {
        public override void Bake(AgentColliderAuthoring authoring)
        {
#if UNITY_ENTITIES_VERSION_65
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new AgentCollider { });
#else
            AddComponent(new AgentCollider { });
#endif
        }
    }
}
