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
        public GameObject _gameobject;
        public Slider _sliderReload;
        protected override void OnCreate()
        {
            _gameobject = GameObject.FindGameObjectWithTag("Reload");
            _sliderReload = _gameobject.GetComponent<Slider>();
            RequireForUpdate<PlayerTag>();
            RequireForUpdate<EquippingWeapon>();
            RequireForUpdate<CharacterAnimatorReference>();
            _gameobject.SetActive(false);
        }
        protected override void OnUpdate()
        {
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
                //var _ammo = SystemAPI.GetComponentRO<MagazineData>(_weapon.Entity);
                _sliderReload.maxValue = _reload.ValueRO.Value;
                if (_isReload.Value.GetBool("Reload_b") == true)
                {
                    _gameobject.SetActive(true);
                    _sliderReload.value += SystemAPI.Time.DeltaTime;
                }
                if (_isReload.Value.GetBool("Reload_b") == false)
                {
                    _gameobject.SetActive(false);
                    _sliderReload.value = 0;
                }
            };
        }
    }
}