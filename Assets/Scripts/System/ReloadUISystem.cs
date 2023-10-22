using AxieRescuer;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;
namespace AxieRescuer
{
    [BurstCompile]
    public partial class ReloadUISystem : SystemBase
    {
        private GameObject _gameobject;
        private Slider _sliderReload;
        private GameObject _slider;
        protected override void OnCreate()
        {

            RequireForUpdate<PlayerTag>();
            RequireForUpdate<EquippingWeapon>();
            RequireForUpdate<CharacterAnimatorReference>();

        }
        protected override void OnUpdate()
        {
            _gameobject = GameObject.FindGameObjectWithTag("Reload");
            _slider = _gameobject.transform.GetChild(0).gameObject;
            if (_slider == null) return;
            _sliderReload = _slider.GetComponentInChildren<Slider>();
            var _player = SystemAPI.GetSingletonEntity<PlayerTag>();
            var _transform = SystemAPI.GetComponent<LocalTransform>(_player);
            var _isReload = EntityManager.GetComponentObject<CharacterAnimatorReference>(_player);
            var position = _transform.Position;
            position.y = 10f;
            _sliderReload.transform.position = position;
            var temp = _transform.Scale / 20f;
            _sliderReload.transform.localScale = new Vector3(temp, temp, temp);
            var _weapon = EntityManager.GetComponentObject<EquippingWeapon>(_player);
            if (_weapon.Entity != Entity.Null)
            {
                var _reload = SystemAPI.GetComponentRO<ReloadTime>(_weapon.Entity);
                _sliderReload.maxValue = _reload.ValueRO.Value;
                if (_isReload.Value.GetBool("Reload_b") == true)
                {
                    _slider.SetActive(true);
                    _sliderReload.value += SystemAPI.Time.DeltaTime;
                }
                if (_isReload.Value.GetBool("Reload_b") == false)
                {
                    _slider.SetActive(false);
                    _sliderReload.value = 0;
                }
            };
        }
    }
}