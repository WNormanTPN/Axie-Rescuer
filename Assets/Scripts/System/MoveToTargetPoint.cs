using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;
using UnityEditor.SceneTemplate;

namespace AxieRescuer
{
    [BurstCompile]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class MoveToTargetPoint : SystemBase
    {
        private Camera _camera;
        private float3 _position;
        private bool _flag;
        private float _cameratime = 5;
        private float stop = 1;
        [BurstCompile]
       protected override void OnCreate()
        {
            RequireForUpdate<PlayerTag>();
            RequireForUpdate<FollowOffset>();
            RequireForUpdate<NeedDestroy>();
            RequireForUpdate<TargetPointTag>();
        }
        protected override void OnStartRunning()
        {
            _camera = Camera.main;
            var initialRotation = SystemAPI.GetSingleton<FollowOffset>().Rotation;
            _camera.transform.rotation = Quaternion.Euler(initialRotation);
            _flag = true;   
        }
        protected override void OnUpdate()
        {
            var player = SystemAPI.GetSingletonEntity<PlayerTag>();
            var targetPoint = SystemAPI.GetSingletonEntity<TargetPointTag>();
            var playerTransform = SystemAPI.GetComponent<LocalTransform>(player);
            var positionOffset = SystemAPI.GetSingleton<FollowOffset>().Position;
            var targetPointTransform = SystemAPI.GetComponent<LocalTransform>(targetPoint);
            var endPostition = targetPointTransform.Position;
            endPostition.x = 33;
            float elapsedTime = 0;
            elapsedTime = SystemAPI.Time.DeltaTime;
            if (math.distancesq(playerTransform.Position,targetPointTransform.Position) <= 5)
            {
                if (_flag)
                {

                    _position = targetPointTransform.Position;
                    _flag = false;
                }
                else if (_flag == false) {
                    {
                        if (_cameratime >= 0)
                        {
                            var smoothPostiotion = Vector3.Lerp(_position, endPostition, elapsedTime / 2);
                            _position = smoothPostiotion;
                            _camera.transform.position = smoothPostiotion + (Vector3)positionOffset;
                            _cameratime -= SystemAPI.Time.DeltaTime;
                        }
                        if (_cameratime <= 0)
                        {
                            var smoothPostiotion = Vector3.Lerp(_position, targetPointTransform.Position, elapsedTime / 2);
                            _position = smoothPostiotion;
                            _camera.transform.position = smoothPostiotion + (Vector3)positionOffset;
                            stop += stop * elapsedTime;
                        }
                    }
                }
            }
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            ecb.SetComponentEnabled<NeedDestroy>(targetPoint, true);
            if (stop >= 15)
            {
                ecb.Playback(EntityManager);
            }
        }
    }
}