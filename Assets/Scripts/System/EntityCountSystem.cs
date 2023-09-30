using TMPro;
using Unity.Burst;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace AxieRescuer
{
    public partial struct EntityCountSystem : ISystem
    {
        private int frequency;
        private float timer;
        private int counter;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            frequency = 5;
            timer = 0;
        }

        public void OnUpdate(ref SystemState state)
        {
            timer += SystemAPI.Time.DeltaTime;
            if (timer < 1 / frequency) return;

            timer = 0;
            counter = 0;
            foreach (var s in SystemAPI.Query<Simulate>())
            {
                counter++;
            }
            //Debug.Log((counter + GameObject.FindObjectsOfType<Object>().Length));
        }
    }
}