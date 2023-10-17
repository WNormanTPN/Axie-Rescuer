using System.Collections;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace AxieRescuer
{
    [BurstCompile]
    public partial class RescueAxieSystem : SystemBase
    {
        private Transform axieTransform;
        private float timer;
        private bool isNotifyShowing;
        private GameObject rescuingNotify;

        [BurstCompile]
        protected override void OnCreate()
        {
            timer = 0f;
            isNotifyShowing = false;
            var wildAxieQuery = SystemAPI.QueryBuilder()
                .WithAll<WildAxieTag>()
                .WithAll<CharacterAnimatorReference>()
                .WithAll<LocalTransform>()
                .Build();
            RequireForUpdate(wildAxieQuery);
            RequireForUpdate<PlayerTag>();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            timer += SystemAPI.Time.DeltaTime;
            if(isNotifyShowing && timer > 1)
            {
                timer = 0;
                isNotifyShowing = false;
                if (rescuingNotify != null)
                {
                    var img = rescuingNotify.GetComponent<Image>();
                    img.enabled = false;
                    var tmp = rescuingNotify.GetComponentInChildren<TextMeshProUGUI>();
                    tmp.enabled = false;
                }
            }
            var player = SystemAPI.GetSingletonEntity<PlayerTag>();
            var playerTransform = SystemAPI.GetComponent<LocalTransform>(player);
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach(var (transform, entity) in SystemAPI.Query<LocalTransform>().WithAll<WildAxieTag>().WithEntityAccess())
            {
                if (!EntityManager.IsComponentEnabled<WildAxieTag>(entity)) continue;
                if(math.distance(playerTransform.Position, transform.Position) < 15)
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    var hits = Physics.RaycastAll(ray, 1000f);
                    bool isNotHoveringAxie = true;
                    var followingAxie = SystemAPI.GetComponent<FollowingAxie>(player);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].transform.gameObject.layer != LayerMask.NameToLayer("Axie")) continue;
                        isNotHoveringAxie = false;
                        axieTransform = hits[i].transform;
                        var isHoveringInFollowingAxie = false;
                        if(EntityManager.IsComponentEnabled<FollowingAxie>(player))
                        {
                            var followingAxieTransform = EntityManager.GetComponentObject<CharacterAnimatorReference>(followingAxie.Entity).Value.gameObject.transform;
                            if(followingAxieTransform == axieTransform)
                            {
                                isHoveringInFollowingAxie = true;
                            }
                        }
                        if (!isHoveringInFollowingAxie)
                        {
                            axieTransform.localScale = new Vector3(6, 6, 6);
                        }
                        // Selection
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (EntityManager.IsComponentEnabled<FollowingAxie>(player))
                            {
                                rescuingNotify = GameObject.FindGameObjectWithTag("RescuingNotify");
                                var img = rescuingNotify.GetComponent<Image>();
                                img.enabled = true;
                                var tmp = rescuingNotify.GetComponentInChildren<TextMeshProUGUI>();
                                tmp.enabled = true;
                                isNotifyShowing = true;
                                timer = 0;
                                continue;
                            }
                            axieTransform.localScale = new Vector3(4, 4, 4);
                            EntityManager.SetComponentEnabled<WildAxieTag>(entity, false);
                            EntityManager.SetComponentData(player, new FollowingAxie
                            {
                                Entity = entity,
                            });
                            EntityManager.SetComponentEnabled<FollowingAxie>(player, true);
                        }
                    }
                    if(isNotHoveringAxie && axieTransform != null)
                    {
                        axieTransform.localScale = new Vector3(4, 4, 4);
                    }
                }
            }
            ecb.Playback(EntityManager);
        }

        private IEnumerable closeNotify(float duration, GameObject notify)
        {
            yield return new WaitForSeconds(duration);
            var img = notify.GetComponent<Image>();
            img.enabled = false;
            var tmp = notify.GetComponentInChildren<TextMeshPro>();
            tmp.enabled = false;
        }
    }
}