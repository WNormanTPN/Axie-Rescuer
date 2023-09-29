using AxieRescuer;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ZombieAuthoring : MonoBehaviour
{
    public class ZombieBaker : Baker<ZombieAuthoring>
    {
        public override void Bake(ZombieAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<ZombieTag>(entity);
            AddComponent(entity, new Health
            {
                Max = 10,
                Current = 10,
            });
            AddBuffer<DamageReceived>(entity);
            AddComponent<IsDie>(entity);
            SetComponentEnabled<IsDie>(entity, false);

            AddComponent(entity, new FindTargetComponents
            {
                onRange = false
            });
            AddComponent(entity, new DropRate
            {
                Value = 0.05f
            });
            AddComponent(entity, new NeedDestroy
            {
                CountdownTime = 1,
            });
            SetComponentEnabled<NeedDestroy>(entity, false);

        }
    }
}
