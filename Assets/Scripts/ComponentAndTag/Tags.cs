using Unity.Entities;

namespace AxieRescuer
{
    public struct PlayerTag : IComponentData { }
    public struct ZombieTag : IComponentData { }
    public struct BuildingTag : IComponentData { }
    public struct IsDie : IComponentData, IEnableableComponent { }
}