using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace AxieRescuer
{
    public class AxieAuthoring : MonoBehaviour
    {
        public class AxieBaker : Baker<AxieAuthoring>
        {
            public override void Bake(AxieAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<AxieNeedInitTag>(entity);
                SetComponentEnabled<AxieNeedInitTag>(entity, true);
            }
        }
    }
}
