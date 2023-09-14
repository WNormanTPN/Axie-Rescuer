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
            var readInputEntity = SystemAPI.GetSingletonEntity<MoveInput>();
            var moveInput = _input.Player.Move.ReadValue<Vector2>();
            var lookInput = _input.Player.Look.ReadValue<Vector2>();
            _input.Player.Fire.started += (InputAction.CallbackContext context) =>
            {
                EntityManager.SetComponentEnabled<FireInput>(readInputEntity, true);
            };
            EntityManager.SetComponentData(readInputEntity, new MoveInput
            {
                Value = moveInput,
            });
            EntityManager.SetComponentData(readInputEntity, new LookInput
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