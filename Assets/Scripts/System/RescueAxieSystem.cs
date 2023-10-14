using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace AxieRescuer
{
    [BurstCompile]
    public partial class RescueAxieSystem : SystemBase
    {
        private Transform axieTransform;

        [BurstCompile]
        protected override void OnCreate()
        {
            var wildAxieQuery = SystemAPI.QueryBuilder()
                .WithAll<WildAxieTag>()
                .WithAll<CharacterAnimatorReference>()
                .WithAll<LocalTransform>()
                .Build();
            RequireForUpdate(wildAxieQuery);
            RequireForUpdate<PlayerTag>();
        }

        protected override void OnUpdate()
        {
            var player = SystemAPI.GetSingletonEntity<PlayerTag>();
            var playerTransform = SystemAPI.GetComponent<LocalTransform>(player);
            foreach(var (transform, entity) in SystemAPI.Query<LocalTransform>().WithAll<WildAxieTag>().WithEntityAccess())
            {
                if(math.distance(playerTransform.Position, transform.Position) < 10)
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    var hits = Physics.RaycastAll(ray, 1000f);
                    bool flag = true;
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].transform.gameObject.layer != LayerMask.NameToLayer("Axie")) continue;
                        flag = false;
                        axieTransform = hits[i].transform;
                        axieTransform.localScale = new Vector3(6, 6, 6);
                        // Selection
                        if (Input.GetMouseButtonDown(0))
                        {
                            axieTransform.localScale = new Vector3(4, 4, 4);
                            EntityManager.SetComponentEnabled<WildAxieTag>(entity, false);
                        }
                    }
                    if(flag && axieTransform != null)
                    {
                        axieTransform.localScale = new Vector3(4, 4, 4);
                    }
                }
            }
        }
    }
}