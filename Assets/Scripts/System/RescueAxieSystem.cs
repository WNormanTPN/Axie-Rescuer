using System.Collections;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Device;
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
                if(math.distance(playerTransform.Position, transform.Position) < 25)
                {
                    #region Desktop
                    if (UnityEngine.SystemInfo.deviceType == DeviceType.Desktop)
                    {
                        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        var hits = Physics.RaycastAll(ray, 300f);
                        bool isNotHoveringAxie = true;
                        var followingAxie = SystemAPI.GetComponent<FollowingAxie>(player);
                        for (int i = 0; i < hits.Length; i++)
                        {
                            if (hits[i].transform.gameObject.layer != LayerMask.NameToLayer("Axie")) continue;
                            isNotHoveringAxie = false;
                            axieTransform = hits[i].transform;
                            var axieObject = axieTransform.gameObject;
                            var axieAnimator = EntityManager.GetComponentObject<CharacterAnimatorReference>(entity);
                            if (axieObject != axieAnimator.Value.gameObject) continue;
                            var isHoveringInFollowingAxie = false;
                            if (EntityManager.IsComponentEnabled<FollowingAxie>(player))
                            {
                                if (!EntityManager.IsComponentEnabled<FollowingAxie>(player)) return;
                                var followingAxieTransform = EntityManager.GetComponentObject<CharacterAnimatorReference>(followingAxie.Entity).Value.gameObject.transform;
                                if (followingAxieTransform == axieTransform)
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
                                axieTransform.localScale = new Vector3(8, 8, 8);
                            }
                            if (Input.GetMouseButtonUp(0))
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
                                var outline = axieObject.GetComponent<Outline>();
                                outline.OutlineColor = Color.green;
                                EntityManager.SetComponentEnabled<WildAxieTag>(entity, false);
                                EntityManager.SetComponentData(player, new FollowingAxie
                                {
                                    Entity = entity,
                                });
                                EntityManager.SetComponentEnabled<FollowingAxie>(player, true);
                            }
                        }
                        if (isNotHoveringAxie && axieTransform != null)
                        {
                            axieTransform.localScale = new Vector3(4, 4, 4);
                        }
                    }
                    #endregion

                    #region Mobile
                    if (UnityEngine.SystemInfo.deviceType == DeviceType.Handheld)
                    {
                        if (Input.touchCount > 0)
                        {
                            for (int j = 0; j < Input.touchCount; j++)
                            {
                                var touch = Input.GetTouch(j);
                                var ray = Camera.main.ScreenPointToRay(touch.position);
                                var hits = Physics.RaycastAll(ray, 300f);
                                bool isNotHoveringAxie = true;
                                var followingAxie = SystemAPI.GetComponent<FollowingAxie>(player);
                                for (int i = 0; i < hits.Length; i++)
                                {
                                    if (hits[i].transform.gameObject.layer != LayerMask.NameToLayer("Axie")) continue;
                                    isNotHoveringAxie = false;
                                    axieTransform = hits[i].transform;
                                    var axieObject = axieTransform.gameObject;
                                    var isHoveringInFollowingAxie = false;
                                    if (EntityManager.IsComponentEnabled<FollowingAxie>(player))
                                    {
                                        var followingAxieTransform = EntityManager.GetComponentObject<CharacterAnimatorReference>(followingAxie.Entity).Value.gameObject.transform;
                                        if (followingAxieTransform == axieTransform)
                                        {
                                            isHoveringInFollowingAxie = true;
                                        }
                                    }
                                    if (!isHoveringInFollowingAxie)
                                    {
                                        axieTransform.localScale = new Vector3(6, 6, 6);
                                    }

                                    // Selection

                                    if (touch.phase == TouchPhase.Ended)
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
                                        var outline = axieObject.GetComponent<Outline>();
                                        outline.OutlineColor = Color.green;
                                        EntityManager.SetComponentEnabled<WildAxieTag>(entity, false);
                                        EntityManager.SetComponentData(player, new FollowingAxie
                                        {
                                            Entity = entity,
                                        });
                                        EntityManager.SetComponentEnabled<FollowingAxie>(player, true);
                                    }
                                }
                                if (isNotHoveringAxie && axieTransform != null)
                                {
                                    axieTransform.localScale = new Vector3(4, 4, 4);
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            ecb.Playback(EntityManager);
        }
    }
}