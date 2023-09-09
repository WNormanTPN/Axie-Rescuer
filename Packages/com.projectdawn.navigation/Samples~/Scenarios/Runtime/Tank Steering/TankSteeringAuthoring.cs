using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using ProjectDawn.Navigation.Hybrid;

namespace ProjectDawn.Navigation.Sample.Scenarios
{
    [RequireComponent(typeof(AgentAuthoring))]
    [DisallowMultipleComponent]
    public class TankSteeringAuthoring : MonoBehaviour
    {
        [SerializeField]
        float Speed = 3.5f;

        [SerializeField]
        float Acceleration = 8;

        [SerializeField]
        float AngularSpeed = 120;

        [SerializeField]
        float StoppingDistance = 0;

        [SerializeField]
        bool AutoBreaking = true;

        Entity m_Entity;

        /// <summary>
        /// Returns default component of <see cref="TankSteering"/>.
        /// </summary>
        public TankSteering DefaultSteering => new TankSteering
        {
            Speed = Speed,
            Acceleration = Acceleration,
            AngularSpeed = math.radians(AngularSpeed),
            StoppingDistance = StoppingDistance,
            AutoBreaking = AutoBreaking,
        };

        void Awake()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            m_Entity = GetComponent<AgentAuthoring>().GetOrCreateEntity();
            world.EntityManager.AddComponentData(m_Entity, DefaultSteering);
        }

        void OnDestroy()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world != null)
                world.EntityManager.RemoveComponent<TankSteering>(m_Entity);
        }
    }

    internal class TankSteeringBaker : Baker<TankSteeringAuthoring>
    {
        public override void Bake(TankSteeringAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), authoring.DefaultSteering);
        }
    }
}
