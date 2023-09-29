using AxieRescuer;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace AxieRescuer
{ 
public class ResourcesAuthoring : MonoBehaviour
{
    public List<GameObject> WeaponList = new List<GameObject>();
    public class ResourcesBaker : Baker<ResourcesAuthoring>
    {
            public override void Bake(ResourcesAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddBuffer<WeaponsBuffer>(entity);
                foreach (var weaponObject in authoring.WeaponList)
                {
                    AppendToBuffer(entity, new WeaponsBuffer
                    {
                        Value = GetEntity(weaponObject, TransformUsageFlags.Dynamic),
                    });
                }

            }
        }
    }
}