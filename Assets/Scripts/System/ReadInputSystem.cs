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
        private Entity _readInputEntity;

        protected override void OnCreate()
        {
            _input = new PlayerInput();
            RequireForUpdate<MoveInput>();
            RequireForUpdate<LookInput>();
        }

        protected override void OnStartRunning()
        {
            _readInputEntity = SystemAPI.GetSingletonEntity<MoveInput>();
            _input.Enable();
            _input.Player.Fire.started += (InputAction.CallbackContext context) =>
            {
                EntityManager.SetComponentEnabled<FireInput>(_readInputEntity, true);
            };
            _input.Player.Fire.canceled += (InputAction.CallbackContext context) =>
            {
                EntityManager.SetComponentEnabled<FireInput>(_readInputEntity, false);
            };
        }

        protected override void OnUpdate()
        {
            var moveInput = _input.Player.Move.ReadValue<Vector2>();
            var lookInput = _input.Player.Look.ReadValue<Vector2>();
            EntityManager.SetComponentData(_readInputEntity, new MoveInput
            {
                Value = moveInput,
            });
            EntityManager.SetComponentData(_readInputEntity, new LookInput
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