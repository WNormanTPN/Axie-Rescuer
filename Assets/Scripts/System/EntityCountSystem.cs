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
            frequency = 4;
            timer = 0;
        }

        public void OnUpdate(ref SystemState state)
        {
            timer += SystemAPI.Time.DeltaTime;
            if (timer < (1.0 / frequency)) return;

            timer = 0;
            counter = 0;
            foreach (var s in SystemAPI.Query<Simulate>())
            {
                counter++;
            }
            counter += GameObject.FindObjectsOfType<Object>().Length;
            var entityCountUI = GameObject.FindGameObjectWithTag("EntityCount");
            if (entityCountUI == null) return;
            var tmp = entityCountUI.GetComponent<TextMeshProUGUI>();
            tmp.text = "Entities: " + counter;
        }
    }
}