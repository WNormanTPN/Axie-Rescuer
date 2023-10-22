using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Collider = Unity.Physics.Collider;
using Random = Unity.Mathematics.Random;
using SphereCollider = Unity.Physics.SphereCollider;

namespace AxieRescuer
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(ReadInputSystem))]
    public partial struct FireSystem : ISystem
    {
        private bool _isReloading;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _isReloading = false;
            state.RequireForUpdate<FireInput>();
            state.RequireForUpdate<EquippingWeapon>();
            state.RequireForUpdate<RandomSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var player = SystemAPI.GetSingletonEntity<PlayerTag>();
            if (!state.EntityManager.HasComponent<EquippingWeapon>(player)) return;
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
                if (magazineData.ValueRO.TotalValue > 0)
                {
                    if (magazineData.ValueRO.CurrentValue > 0) // If projectile count > 0
                    {
                        var readInputEntity = SystemAPI.GetSingletonEntity<MoveInput>();
                        if (state.EntityManager.IsComponentEnabled<FireInput>(readInputEntity))
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
                                        StartPos = playerTransform.Position + math.up() * 2 + playerTransform.Forward() * 1,
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
                #endregion

                #region Reload
                if (_isReloading)
                {
                    reloadTime.ValueRW.Timer += SystemAPI.Time.DeltaTime;
                    if (reloadTime.ValueRO.Timer >= reloadTime.ValueRO.Value)
                    {
                        _isReloading = false;
                        magazineData.ValueRW.CurrentValue = magazineData.ValueRO.MaxValuePerReload;
                        animatorReference.Value.SetBool("Reload_b", false);
                        reloadTime.ValueRW.Timer = 0;
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
            switch (WeaponType.Value)
            {
                case WeaponTypeEnum.Handgun:
                    ShootThrough(1.5f);
                    break;
                case WeaponTypeEnum.Rifle:
                    ShootThrough(1.7f);
                    break;
                case WeaponTypeEnum.Shotgun:
                    var projectileCount = Random.NextInt(5, 10);
                    for (int i = 0; i < projectileCount; i++)
                    {
                        ShootWithoutPenetration(1f);
                    }

                    break;
                default:
                    ShootWithoutPenetration(1.2f);

                    break;
            }
        }

        [BurstCompile]
        public unsafe void ShootThrough(float projectileRadius)
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
            SphereGeometry sphereGeometry = new SphereGeometry() { Center = float3.zero, Radius = projectileRadius };
            BlobAssetReference<Collider> sphereCollider = SphereCollider.Create(sphereGeometry, filter);
            var rayCastInput = new ColliderCastInput
            {
                Collider = (Collider*)sphereCollider.GetUnsafePtr(),
                Start = StartPos,
                End = endPos,
            };
            var hits = new NativeList<ColliderCastHit>(Allocator.Temp);
            World.CastCollider(rayCastInput, ref hits);
            var tempHits = new NativeList<ColliderCastHit>(Allocator.Temp);
            tempHits.CopyFrom(hits);
            hits.Clear();
            ECB.AppendToBuffer(DrawTrajectorySingleton, new Trajectory
            {
                Start = StartPos,
                End = endPos,
                Width = projectileRadius / 4,
                ShowTime = 0.1f,
            });
            for (int i = tempHits.Length - 1; i >= 0; i--)
            {
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
            hits.Dispose();
            tempHits.Dispose();
        }

        [BurstCompile]
        public unsafe void ShootWithoutPenetration(float projectileRadius)
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
            SphereGeometry sphereGeometry = new SphereGeometry() { Center = float3.zero, Radius = projectileRadius };
            BlobAssetReference<Collider> sphereCollider = SphereCollider.Create(sphereGeometry, filter);
            var rayCastInput = new ColliderCastInput
            {
                Collider = (Collider*)sphereCollider.GetUnsafePtr(),
                Start = StartPos,
                End = endPos,
            };
            var hits = new NativeList<ColliderCastHit>(Allocator.Temp);
            World.CastCollider(rayCastInput, ref hits);

            var isHit = GetClosestHitPosition(EntityManager, StartPos, hits, out var closestHit);
            if (isHit)
            {
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
                Width = projectileRadius / 4,
                ShowTime = 0.1f,
            });
            hits.Dispose();
        }

        [BurstCompile]
        public bool GetClosestHitPosition(EntityManager entityManager, float3 startPos, NativeList<ColliderCastHit> hits, out ColliderCastHit closestHit)
        {
            if (hits.Length == 0)
            {
                closestHit = new ColliderCastHit(); // Provide a default value or handle the case where there are no hits.
                return false; // No hits to process.
            }

            ColliderCastHit? validClosestHit = null;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < hits.Length; i++)
            {
                ColliderCastHit currentHit = hits[i];
                Entity hitEntity = currentHit.Entity;

                if (entityManager.HasComponent<ZombieTag>(hitEntity) ||
                    entityManager.HasComponent<BuildingTag>(hitEntity))
                {
                    float currentDistance = math.length(currentHit.Position - startPos);

                    if (currentDistance < closestDistance)
                    {
                        closestDistance = currentDistance;
                        validClosestHit = currentHit;
                    }
                }
            }

            if (validClosestHit.HasValue)
            {
                closestHit = validClosestHit.Value;
                return true; // Valid closest hit found.
            }
            else
            {
                closestHit = new ColliderCastHit(); // Provide a default value when no valid hits are found.
                return false; // No valid hits found.
            }
        }
    }
}