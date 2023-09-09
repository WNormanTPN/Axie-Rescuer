using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Logging;
using Unity.Mathematics;
using Unity.Transforms;

namespace AxieRescuer
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(ReadInputSystem))]
    public partial struct PlayerMovementSystem : ISystem
    {
        private EntityQuery _playerQuery;
        private MoveInput _moveInput;
        private LookInput _lookInput;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _playerQuery = SystemAPI.QueryBuilder()
                .WithAll<PlayerTag>()
                .WithAll<MoveDirection>()
                .WithAll<TargetRotationDirection>()
                .Build();
            state.RequireForUpdate(_playerQuery);
            state.RequireForUpdate<MoveInput>();
            state.RequireForUpdate<LookInput>();
            state.RequireForUpdate<FollowOffset>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _moveInput = SystemAPI.GetSingleton<MoveInput>();
            _lookInput = SystemAPI.GetSingleton<LookInput>();
            var cameraRotation = SystemAPI.GetSingleton<FollowOffset>().Rotation;
            new SyncPlayerMovementDataJob
            {
                MoveInputData = _moveInput.Value,
                LookInputData = _lookInput.Value,
                OffsetAngle = cameraRotation.y,
            }.ScheduleParallel(_playerQuery);
        }
    }

    [BurstCompile]
    public partial struct SyncPlayerMovementDataJob : IJobEntity
    {
        public float2 MoveInputData;
        public float2 LookInputData;
        public float OffsetAngle;

        public void Execute
        (
            ref MoveDirection moveDirection,
            ref TargetRotationDirection targetRotation
        )
        {
            OffsetAngle = math.radians(OffsetAngle);

            var x = MoveInputData.x;
            var z = MoveInputData.y;
            moveDirection.Value.x = x * math.cos(OffsetAngle) + z * math.sin(OffsetAngle);
            moveDirection.Value.z = (float)(-x * Math.Sin(OffsetAngle) + z * math.cos(OffsetAngle));

            x = LookInputData.x;
            z = LookInputData.y;
            targetRotation.Value.x = x * math.cos(OffsetAngle) + z * math.sin(OffsetAngle);
            targetRotation.Value.z = (float)(-x * Math.Sin(OffsetAngle) + z * math.cos(OffsetAngle));
        }
    }
}