using AxieRescuer;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace AxieRescuer
{
    public class BuildingAuthoring : MonoBehaviour
    {
        public class BuildingBaker : Baker<BuildingAuthoring>
        {
            public override void Bake(BuildingAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<BuildingTag>(entity);
            }
        }
    }
}
