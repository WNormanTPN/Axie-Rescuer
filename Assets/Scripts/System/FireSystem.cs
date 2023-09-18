using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Logging;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using RaycastHit = Unity.Physics.RaycastHit;

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
        }

        public void OnUpdate(ref SystemState state)
        {
            var inputSingleton = SystemAPI.GetSingletonEntity<MoveDirection>();
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
                                    var range = SystemAPI.GetComponent<Range>(equippingWeapon.Entity);
                                    var damage = SystemAPI.GetComponent<Damage>(equippingWeapon.Entity);
                                    var accuracy = SystemAPI.GetComponent<Accuracy>(equippingWeapon.Entity);
                                    var world = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
                                    var drawtrajectorySingleton = SystemAPI.GetSingletonEntity<Trajectory>();
                                    new FireJob
                                    {
                                        StartPos = playerTransform.Position + math.up() + playerTransform.Forward()*4,
                                        Direction = playerTransform.Forward(),
                                        WeaponType = weaponType,
                                        Range = range,
                                        Damage = damage,
                                        Accuracy = accuracy,
                                        DrawTrajectorySingleton = drawtrajectorySingleton,
                                        World = world,
                                        EntityManager = state.EntityManager,
                                        ECB = ecb,
                                    }.Schedule().Complete();
                                    animatorReference.Value.SetBool("Shoot_b", true);
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
        public WeaponType WeaponType;
        public Range Range;
        public Damage Damage;
        public Accuracy Accuracy;
        public Entity DrawTrajectorySingleton;
        public PhysicsWorldSingleton World;
        public EntityManager EntityManager;
        public EntityCommandBuffer ECB;

        public void Execute()
        {
            switch(WeaponType.Value)
            {
                case WeaponTypeEnum.Bomb:
                    

                    break;
                case WeaponTypeEnum.Shotgun:
                    var projectileCount = new Random(123).NextInt(3, 7);
                    for(int i = 0; i < projectileCount; i++)
                    {
                        Fire();
                    }

                    break;
                default:
                    Fire();
                    
                    break;
            }
        }

        public void Fire()
        {
            var rand = new Random(123);
            //var deviationAngle = 90 * (100 - Accuracy.Value)/100;
            //deviationAngle = math.radians(rand.NextFloat(-deviationAngle, deviationAngle));
            var endPos = Direction;
            endPos.xz *= Range.Value;
            endPos += StartPos;
            //endPos.x = endPos.x * math.cos(deviationAngle) + endPos.z * math.sin(deviationAngle);
            //endPos.z = -endPos.x * math.sin(deviationAngle) + endPos.z * math.cos(deviationAngle);
            var rayCastInput = new RaycastInput
            {
                Start = StartPos,
                End = endPos,
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = ~0u,
                    GroupIndex = 0,
                }
            };
            var hits = new NativeList<RaycastHit>(Allocator.Temp);
            var tempHits = hits;
            hits.Clear();
            World.CastRay(rayCastInput, ref hits);
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
                        Value = rand.NextFloat(Damage.MinValue, Damage.MaxValue),
                    });
                }
            }
            hits.Dispose();
        }
    }
}