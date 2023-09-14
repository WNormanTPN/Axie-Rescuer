using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AxieRescuer
{
    public class WeaponAuthoring : MonoBehaviour
    {
        public GameObject Prefab;
        public WeaponTypeEnum WeaponType;
        public float2 DamageRange;
        public float Range;
        [Range(0, 100)] public int Accuracy;
        public float FireRate;
        public float ReloadTime;
        public int TotalMagazineValue;
        public int MaxValuePerReload;

        public class WeaponBaker : Baker<WeaponAuthoring>
        {
            public override void Bake(WeaponAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponentObject(entity, new WeaponPrefab
                {
                    Value = authoring.Prefab,
                });
                AddComponent(entity, new WeaponType
                {
                    Value = authoring.WeaponType,
                });
                AddComponent(entity, new Damage
                {
                    MaxValue = authoring.DamageRange.x,
                    MinValue = authoring.DamageRange.y,
                });
                AddComponent(entity, new Range
                {
                    Value = authoring.Range,
                });
                AddComponent(entity, new Accuracy
                {
                    Value = authoring.Accuracy,
                });
                AddComponent(entity, new FireRate
                {
                    Value = authoring.FireRate,
                });
                AddComponent(entity, new ReloadTime
                {
                    Value = authoring.ReloadTime,
                });
                AddComponent(entity, new MagazineData
                {
                    TotalValue = authoring.TotalMagazineValue,
                    CurrentValue = authoring.MaxValuePerReload,
                    MaxValuePerReload = authoring.MaxValuePerReload,
                });
            }
        }
    }
}
