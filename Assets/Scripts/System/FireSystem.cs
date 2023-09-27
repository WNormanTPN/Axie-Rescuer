using System.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Logging;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Collider = Unity.Physics.Collider;
using Random = Unity.Mathematics.Random;
using RaycastHit = Unity.Physics.RaycastHit;
using SphereCollider = Unity.Physics.SphereCollider;

namespace AxieRescuer
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(ReadInputSystem))]
    public partial struct FireSystem : ISystem
    {
        private EntityQuery _isFiringQuery;
        private bool _isReloading;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _isReloading = false;
            _isFiringQuery = SystemAPI.QueryBuilder()
                .WithAll<FireInput>()
                .Build();
            state.RequireForUpdate<FireInput>();
            state.RequireForUpdate<EquippingWeapon>();
            state.RequireForUpdate<RandomSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var player = SystemAPI.GetSingletonEntity<PlayerTag>();
            var equippingWeapon = state.EntityManager.GetComponentObject<EquippingWeapon>(player);

            if (equippingWeapon.Entity != Entity.Null)
            {
                var magazineData = SystemAPI.GetComponentRW<MagazineData>(equippingWeapon.Entity);
                var reloadTime = SystemAPI.GetComponentRW<ReloadTime>(equippingWeapon.Entity);
                var fireRate = SystemAPI.GetComponentRW<FireRate>(equippingWeapon.Entity);
                var animatorReference = state.EntityManager.GetComponentObject<CharacterAnimatorReference>(player);
                var weaponType = SystemAPI.GetComponent<WeaponType>(equippingWeapon.Entity);
                var ecb = new EntityCommandBuffer(Allocator.TempJob);
                fireRate.ValueRW.Timer += SystemAPI.Time.DeltaTime;
                
                #region Fire
                if (_isFiringQuery.CalculateEntityCount() > 0)
                {
                    if (magazineData.ValueRO.TotalValue > 0)
                    {
                        if (magazineData.ValueRO.CurrentValue > 0) // If projectile count > 0
                        {
                            if (fireRate.ValueRO.Timer >= 1.0 / fireRate.ValueRO.Value) // if can fire
                            {
                                magazineData.ValueRW.CurrentValue--;
                                magazineData.ValueRW.TotalValue--;
                                fireRate.ValueRW.Timer = 0;

                                if (weaponType.Value == WeaponTypeEnum.Bomb)
                                {
                                    animatorReference.Value.Play("GrenadeThrow");
                                }
                                else
                                {
                                    var playerTransform = SystemAPI.GetComponent<LocalTransform>(player);
                                    var gunFlash = SystemAPI.GetComponent<GunFlash>(equippingWeapon.Entity);
                                    var range = SystemAPI.GetComponent<Range>(equippingWeapon.Entity);
                                    var damage = SystemAPI.GetComponent<Damage>(equippingWeapon.Entity);
                                    var accuracy = SystemAPI.GetComponent<Accuracy>(equippingWeapon.Entity);
                                    var world = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
                                    var drawtrajectorySingleton = SystemAPI.GetSingletonEntity<Trajectory>();
                                    var random = SystemAPI.GetSingleton<RandomSingleton>();
                                    animatorReference.Value.SetBool("Shoot_b", true);
                                    state.Dependency = new FireJob
                                    {
                                        StartPos = playerTransform.Position + math.up() + playerTransform.Forward()*1,
                                        Direction = playerTransform.Forward(),
                                        GunFlash = gunFlash,
                                        WeaponType = weaponType,
                                        Range = range,
                                        Damage = damage,
                                        Accuracy = accuracy,
                                        Random = Random.CreateFromIndex(random.Random.NextUInt()),
                                        DrawTrajectorySingleton = drawtrajectorySingleton,
                                        World = world,
                                        EntityManager = state.EntityManager,
                                        ECB = ecb,
                                    }.Schedule(state.Dependency);
                                    state.Dependency.Complete();
                                }
                            }
                            else
                            {
                                animatorReference.Value.SetBool("Shoot_b", false);

                            }
                        }
                        else // Reload
                        {
                            _isReloading = true;
                            animatorReference.Value.SetBool("Shoot_b", false);
                            animatorReference.Value.SetBool("Reload_b", true);
                        }
                    }
                }

                #endregion

                #region Reload
                if (_isReloading)
                {
                    reloadTime.ValueRW.Timer += SystemAPI.Time.DeltaTime;
                    if (reloadTime.ValueRO.Timer >= reloadTime.ValueRO.Value)
                    {
                        _isReloading = false;
                        magazineData.ValueRW.TotalValue -= magazineData.ValueRO.MaxValuePerReload - magazineData.ValueRO.CurrentValue;
                        magazineData.ValueRW.CurrentValue = magazineData.ValueRO.MaxValuePerReload;
                        animatorReference.Value.SetBool("Reload_b", false);
                    }
                }

                #endregion

                ecb.Playback(state.EntityManager);
                ecb.Dispose();
            }
        }
    }
    
    [BurstCompile]
    public struct FireJob : IJob
    {
        public float3 StartPos;
        public float3 Direction;
        public GunFlash GunFlash;
        public WeaponType WeaponType;
        public Range Range;
        public Damage Damage;
        public Accuracy Accuracy;
        public Random Random;
        public Entity DrawTrajectorySingleton;
        public PhysicsWorldSingleton World;
        public EntityManager EntityManager;
        public EntityCommandBuffer ECB;

        public void Execute()
        {
            Entity gunFlash;
            switch(WeaponType.Value)
            {
                case WeaponTypeEnum.Shotgun:
                    gunFlash = ECB.Instantiate(GunFlash.Entity);
                    ECB.SetComponent(gunFlash, new LocalTransform
                    {
                        Position = GunFlash.Offset + StartPos,
                        Rotation = quaternion.LookRotationSafe(Direction, math.up()),
                        Scale = GunFlash.Scale,
                    });
                    ECB.SetComponent(gunFlash, new NeedDestroy
                    {
                        CountdownTime = 0.05f,
                    });
                    ECB.SetComponentEnabled<NeedDestroy>(gunFlash, true);


                    var projectileCount = Random.NextInt(5, 10);
                    for(int i = 0; i < projectileCount; i++)
                    {
                        ShootWithoutPenetration();
                    }

                    break;
                default:
                    gunFlash = ECB.Instantiate(GunFlash.Entity);
                    ECB.SetComponent(gunFlash, new LocalTransform
                    {
                        Position = GunFlash.Offset + StartPos,
                        Rotation = quaternion.LookRotationSafe(Direction, math.up()),
                        Scale = GunFlash.Scale,
                    });
                    ECB.SetComponent(gunFlash, new NeedDestroy
                    {
                        CountdownTime = 0.05f,
                    });
                    ECB.SetComponentEnabled<NeedDestroy>(gunFlash, true);
                    ShootWithoutPenetration();
                    
                    break;
            }
        }

        [BurstCompile]
        public unsafe void ShootThrough()
        {
            var deviationAngle = 90 * (100 - Accuracy.Value) / 100;
            deviationAngle = math.radians(Random.NextFloat(-deviationAngle, deviationAngle));
            var endPos = Direction;
            endPos.xz *= Range.Value;
            endPos.x = endPos.x * math.cos(deviationAngle) + endPos.z * math.sin(deviationAngle);
            endPos.z = -endPos.x * math.sin(deviationAngle) + endPos.z * math.cos(deviationAngle);
            endPos += StartPos;
            var filter = new CollisionFilter()
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u,
                GroupIndex = 0
            };
            SphereGeometry sphereGeometry = new SphereGeometry() { Center = float3.zero, Radius = 0.5f };
            BlobAssetReference<Collider> sphereCollider = SphereCollider.Create(sphereGeometry, filter);
            var rayCastInput = new ColliderCastInput
            {
                Collider = (Collider*)sphereCollider.GetUnsafePtr(),
                Start = StartPos,
                End = endPos,
            };
            var hits = new NativeList<ColliderCastHit>(Allocator.Temp);
            World.CastCollider(rayCastInput, ref hits);
            var tempHits = hits;
            hits.Clear();
            ECB.AppendToBuffer(DrawTrajectorySingleton, new Trajectory
            {
                Start = StartPos,
                End = endPos,
                ShowTime = 0.1f,
            });
            for (int i = tempHits.Length - 2; i > 0; i--)
            {
                if (EntityManager.HasComponent<BuildingTag>(tempHits[i].Entity))
                {
                    break;
                }
                if (EntityManager.HasComponent<ZombieTag>(tempHits[i].Entity))
                {
                    hits.Add(tempHits[i]);
                }
            }
            for (int i = 0; i < hits.Length; i++)
            {
                if (EntityManager.HasBuffer<DamageReceived>(hits[i].Entity))
                {
                    ECB.AppendToBuffer(hits[i].Entity, new DamageReceived
                    {
                        Value = Random.NextFloat(Damage.MinValue, Damage.MaxValue),
                    });
                }
            }
        }

        [BurstCompile]
        public unsafe void ShootWithoutPenetration()
        {
            var deviationAngle = 90 * (100 - Accuracy.Value) / 100;
            deviationAngle = math.radians(Random.NextFloat(-deviationAngle, deviationAngle));
            var endPos = Direction;
            endPos.xz *= Range.Value;
            endPos.x = endPos.x * math.cos(deviationAngle) + endPos.z * math.sin(deviationAngle);
            endPos.z = -endPos.x * math.sin(deviationAngle) + endPos.z * math.cos(deviationAngle);
            endPos += StartPos;
            var filter = new CollisionFilter()
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u,
                GroupIndex = 0
            };
            SphereGeometry sphereGeometry = new SphereGeometry() { Center = float3.zero, Radius = 0.5f };
            BlobAssetReference<Collider> sphereCollider = SphereCollider.Create(sphereGeometry, filter);
            var rayCastInput = new ColliderCastInput
            {
                Collider = (Collider*)sphereCollider.GetUnsafePtr(),
                Start = StartPos,
                End = endPos,
            };
            var hits = new NativeList<ColliderCastHit>(Allocator.Temp);
            World.CastCollider(rayCastInput, ref hits);
            if (hits.Length > 1)
            {
                var closestHit = hits[hits.Length - 2];
                if (EntityManager.HasComponent<ZombieTag>(closestHit.Entity))
                {
                    ECB.AppendToBuffer(closestHit.Entity, new DamageReceived
                    {
                        Value = Random.NextFloat(Damage.MinValue, Damage.MaxValue),
                    });
                    endPos = closestHit.Position;
                }
                else if (EntityManager.HasComponent<BuildingTag>(closestHit.Entity))
                {
                    endPos = closestHit.Position;
                }
            }
            ECB.AppendToBuffer(DrawTrajectorySingleton, new Trajectory
            {
                Start = StartPos,
                End = endPos,
                ShowTime = 0.1f,
            });
        }
    }
}