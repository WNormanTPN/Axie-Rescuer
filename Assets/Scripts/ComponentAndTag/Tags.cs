using Unity.Entities;

namespace AxieRescuer
{
    public struct PlayerTag : IComponentData { }
    public struct ZombieTag : IComponentData { }
    public struct ZombieNeedInitTag : IComponentData, IEnableableComponent { }
    public struct BuildingTag : IComponentData { }
    public struct IsDie : IComponentData, IEnableableComponent { }
    public struct DroppedItem : IComponentData, IEnableableComponent { }
    public struct SpawnEnemyTag : IComponentData { }
    public struct AxieTag : IComponentData { }
    public struct AxieNeedInitTag : IComponentData, IEnableableComponent { }
    public struct WildAxieTag : IComponentData, IEnableableComponent { }
}