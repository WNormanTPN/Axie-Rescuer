using System;
using Unity.Entities;
using UnityEngine;

namespace AxieRescuer
{
    public class EquippingWeapon : ICleanupComponentData
    {
        public Entity Entity;
        public GameObject Object;
    }
}