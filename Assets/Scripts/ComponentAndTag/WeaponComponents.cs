using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AxieRescuer
{
    public struct InitialWeapon : IComponentData, IEnableableComponent
    {
        public Entity WeaponEntity;
    }
    public class WeaponPrefab : IComponentData
    {
        public GameObject Value;
    }
    public struct GunFlash : IComponentData
    {
        public Entity Entity;
        public float3 Offset;
        public float Scale;
    }
    public struct Damage : IComponentData
    {
        public float MinValue;
        public float MaxValue;
    }
    public struct Range : IComponentData
    {
        public float Value;
    }
    public struct Accuracy : IComponentData
    {
        public float Value;
    }
    public struct FireRate : IComponentData
    {
        public float Value;
        public float Timer;
    }
    public struct ReloadTime : IComponentData
    {
        public float Value;
        public float Timer;
    }
    public struct MagazineData : IComponentData
    {
        public int TotalValue;
        public int MaxValuePerReload;
        public int CurrentValue;
    }
    public struct WeaponType : IComponentData
    {
        public WeaponTypeEnum Value;
    }
}