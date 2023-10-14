using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace AxieRescuer
{
    public class SafeZoneAuthoring : MonoBehaviour
    {
        public float Radius;

        public class SafeZoneBaker : Baker<SafeZoneAuthoring>
        {
            public override void Bake(SafeZoneAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SafeZone { Radius = authoring.Radius });
            }
        }
    }
}
