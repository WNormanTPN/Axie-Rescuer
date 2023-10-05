using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace AxieRescuer
{
    public class CharacterAnimatorAuthoring : MonoBehaviour
    {
        public GameObject CharacterPrefab;

        public class CharacterAnimatorBaker : Baker<CharacterAnimatorAuthoring>
        {
            public override void Bake(CharacterAnimatorAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponentObject(entity, new CharacterGameObjectPrefab
                {
                    Value = authoring.CharacterPrefab,
                });
            }
        }
    }
}