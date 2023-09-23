using Unity.Entities;

namespace AxieRescuer
{
    public struct NeedDestroy : IComponentData, IEnableableComponent
    {
        public float CountdownTime;
    }
}