using AxieRescuer;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GunFlashAuthoring : MonoBehaviour
{
    public class GunFlashBaker : Baker<GunFlashAuthoring>
    {
        public override void Bake(GunFlashAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<NeedDestroy>(entity);
            SetComponentEnabled<NeedDestroy>(entity, false);
        }
    }
}
