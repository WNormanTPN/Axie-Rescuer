using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AxieRescuer
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class ReadInputSystem : SystemBase
    {
        private PlayerInput _input;

        protected override void OnCreate()
        {
            _input = new PlayerInput();
            RequireForUpdate<MoveInput>();
            RequireForUpdate<LookInput>();
        }

        protected override void OnStartRunning()
        {
            _input.Enable();
        }

        protected override void OnUpdate()
        {
            var moveInput = _input.Player.Move.ReadValue<Vector2>();
            var lookInput = _input.Player.Look.ReadValue<Vector2>();
            SystemAPI.SetSingleton(new MoveInput
            {
                Value = moveInput,
            });
            SystemAPI.SetSingleton(new LookInput
            {
                Value = lookInput,
            });
        }

        protected override void OnStopRunning()
        {
            _input.Disable();
        }

    }
}