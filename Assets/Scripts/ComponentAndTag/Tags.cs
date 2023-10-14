using Unity.Entities;

namespace AxieRescuer
{
    public struct PlayerTag : IComponentData { }
    public struct ZombieTag : IComponentData { }
    public struct BuildingTag : IComponentData { }
    public struct IsDie : IComponentData, IEnableableComponent { }
    public struct DroppedItem : IComponentData, IEnableableComponent { }
    public struct SpawnEnemyTag : IComponentData { }
    public struct ZombieNeedInitTag : IComponentData, IEnableableComponent { }
    public struct WeaponNeedInitTag : IComponentData, IEnableableComponent { }
    public struct TargetPointTag : IComponentData { }
}