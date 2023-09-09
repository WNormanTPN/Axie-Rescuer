using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace AxieRescuer
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct MovementSystem : ISystem
    {
        private EntityQuery _movementComponentQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _movementComponentQuery = SystemAPI.QueryBuilder()
                .WithAll<PhysicsVelocity>()
                .WithAll<MoveDirection>()
                .WithAll<MoveSpeed>()
                .WithAll<TargetRotationDirection>()
                .WithAll<RotationSpeed>()
                .Build();
            state.RequireForUpdate(_movementComponentQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new MovementJob().ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct MovementJob : IJobEntity
    {
        public void Execute
        (
            ref PhysicsVelocity velocity,
            ref LocalTransform transform,
            in MoveSpeed moveSpeed,
            in MoveDirection moveDirection,
            in RotationSpeed rotationSpeed,
            in TargetRotationDirection targetRotation
        )
        {
            // Move
            velocity.Linear = math.lerp(velocity.Linear, moveSpeed.Value * moveDirection.Value, 0.1f);
            //transform.Position += moveDirection.Value * moveSpeed.Value * 0.01f;

            // Rotate
            var curForward = transform.Forward();
            var newForward = math.lerp(curForward, targetRotation.Value, rotationSpeed.Value);
            transform.Rotation = quaternion.LookRotation(newForward, math.up());
        }
    }
}