using AxieRescuer;
using Unity.Entities;
using UnityEngine;

public class RandomSingletonAuthoring : MonoBehaviour
{
    public class RandomSingletonBaker : Baker<RandomSingletonAuthoring>
    {
        public override void Bake(RandomSingletonAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new RandomSingleton
            {
                Random = new Unity.Mathematics.Random(),
            });
        }
    }
}
