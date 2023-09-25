using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AxieRescuer
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderLast = true)]
    public partial struct AudioManageSystem : ISystem
    {
        private EntityQuery _footStepQuery;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _footStepQuery = SystemAPI.QueryBuilder()
                .WithAll<PlayerTag>()
                .WithAll<MoveDirection>()
                .WithAll<FootStepAudio>()
                .Build();
            state.RequireForUpdate(_footStepQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //new FootStepAudioJob().Run(_footStepQuery);
        }
    }

    public partial struct FootStepAudioJob : IJobEntity
    {
        public void Execute(in MoveDirection moveDirection, FootStepAudio audio)
        {
            var playerObject = StaticGameObjectReference.Player;
            if (playerObject != null)
            {
                var audioSource = StaticGameObjectReference.Player.GetComponent<AudioSource>();
                if (float3.zero.Equals(moveDirection.Value))
                {
                    if (audioSource.isPlaying)
                    {
                        audioSource.Stop();
                    }
                }
                else if (!float3.zero.Equals(moveDirection.Value) && !audioSource.isPlaying)
                {
                    audioSource.clip = audio.Value;
                    audioSource.Play();
                }
            }
        }
    }
}