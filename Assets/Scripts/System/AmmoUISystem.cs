using AxieRescuer;
using TMPro;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
namespace AxieRescuer
{ 
[BurstCompile]
    public partial class AmmoUISystem : SystemBase
    {
        private GameObject _text;
        private TMP_Text _textMeshPro;
        protected override void OnCreate()
        {
            RequireForUpdate<PlayerTag>();
            RequireForUpdate<EquippingWeapon>();
        }
        protected override void OnUpdate()
        {
            _text = GameObject.FindGameObjectWithTag("Ammo");
            _textMeshPro = _text.GetComponent<TMP_Text>();
            if (_textMeshPro == null) return;
            var _player = SystemAPI.GetSingletonEntity<PlayerTag>();
            if (!EntityManager.HasComponent<EquippingWeapon>(_player)) return;
            var _weapon = EntityManager.GetComponentObject<EquippingWeapon>(_player);
            if (_weapon.Entity != Entity.Null)
            {
                var _ammo = SystemAPI.GetComponentRO<MagazineData>(_weapon.Entity);
                _textMeshPro.text = _ammo.ValueRO.CurrentValue.ToString() + "/" + _ammo.ValueRO.TotalValue.ToString();
            }
        }
    }
}