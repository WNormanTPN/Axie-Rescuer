using AxieRescuer;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
namespace AxieRescuer
{
    [BurstCompile]
    public partial class HealthBarSystem : SystemBase
    {
        private GameObject _heatlhBar;
        private Slider _slider;
        protected override void OnCreate()
        {
            RequireForUpdate<PlayerTag>();
        }
        protected override void OnUpdate()
        {
            _heatlhBar = GameObject.FindGameObjectWithTag("Health");
            _slider = _heatlhBar.GetComponent<Slider>();
            if (_heatlhBar == null) return;
            var player = SystemAPI.GetSingletonEntity<PlayerTag>();
            var healthValue = SystemAPI.GetComponentRW<Health>(player);
            _slider.maxValue = healthValue.ValueRO.Max;
            _slider.minValue = 0;
            if (healthValue.ValueRO.Current >= 0)
            {
                _slider.value = _slider.maxValue - healthValue.ValueRO.Current;
            }
            else
            {
                _slider.value = 100;
            }
        }
    }
}