using Unity.Entities;
using UnityEngine;

namespace AxieRescuer
{
    public class CharacterGameObjectPrefab : IComponentData
    {
        public GameObject Value;
    }
    public class CharacterAnimatorReference : ICleanupComponentData
    {
        public Animator Value;
    }
}