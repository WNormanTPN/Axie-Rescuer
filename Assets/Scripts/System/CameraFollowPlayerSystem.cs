using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace AxieRescuer
{
    [BurstCompile]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class CameraFollowPlayerSystem : SystemBase
    {
        private Camera _mainCamera;

        [BurstCompile]
        protected override void OnCreate()
        {
            RequireForUpdate<PlayerTag>();
            RequireForUpdate<FollowOffset>();
        }

        protected override void OnStartRunning()
        {
            _mainCamera = Camera.main;
            var initialRotation = SystemAPI.GetSingleton<FollowOffset>().Rotation;
            _mainCamera.transform.rotation = Quaternion.Euler(initialRotation);
        }

        protected override void OnUpdate()
        {
            var player = SystemAPI.GetSingletonEntity<PlayerTag>();
            var playerTransform = SystemAPI.GetComponentRO<LocalTransform>(player).ValueRO;
            var positionOffset = SystemAPI.GetSingleton<FollowOffset>().Position;

            _mainCamera.transform.position = playerTransform.Position + positionOffset;
        }
    }
}