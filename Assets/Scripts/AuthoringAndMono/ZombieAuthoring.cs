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
            AddComponent<ZombieNeedInitTag>(entity);
            SetComponentEnabled<ZombieNeedInitTag>(entity, true);
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
                Value = 0.01f,
            });
            AddComponent(entity, new NeedDestroy
            {
                CountdownTime = 0f,
            });
            SetComponentEnabled<NeedDestroy>(entity, false);
        }
    }
}
