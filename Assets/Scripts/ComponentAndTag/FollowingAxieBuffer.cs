using Unity.Entities;

namespace AxieRescuer
{
    public struct FollowingAxie : IComponentData, IEnableableComponent
    {
        public Entity Entity;
    }
}