using UnityEngine;
using ProjectDawn.Navigation.Hybrid;
using Unity.Entities;
using Unity.Mathematics;
using ProjectDawn.Navigation;

public class AgentSetDestination : MonoBehaviour
{
    public Transform Target;
    void Start()
    {
        GetComponent<AgentAuthoring>().SetDestination(Target.position);
    }
}

// ECS component
public struct SetDestination : IComponentData
{
    public float3 Value;
}

// Bakes mono component into ecs component
class AgentSetDestinationBaker : Baker<AgentSetDestination>
{
    public override void Bake(AgentSetDestination authoring)
    {
        AddComponent(GetEntity(TransformUsageFlags.Dynamic),
            new SetDestination { Value = authoring.Target.position });
    }
}

// Sets agents destination
partial struct AgentSetDestinationSystem : ISystem
{
    public void OnUpdate(ref SystemState systemState)
    {
        foreach (var (destination, body) in SystemAPI.Query<RefRO<SetDestination>, RefRW<AgentBody>>())
        {
            body.ValueRW.SetDestination(destination.ValueRO.Value);
        }
    }
}
