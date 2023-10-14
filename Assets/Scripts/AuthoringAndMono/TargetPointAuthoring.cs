using Unity.Entities;
using UnityEngine;

namespace AxieRescuer
{
    public class TargetPointAuthoring : MonoBehaviour
    {
        public class TargetPointBaker : Baker<TargetPointAuthoring> 
        {
            public override void Bake(TargetPointAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<TargetPointTag>(entity);
                AddComponent(entity, new NeedDestroy
                {
                    CountdownTime = 1,
                });
                SetComponentEnabled<NeedDestroy>(entity, false);
            }
        }
    }
}