using TMPro;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace AxieRescuer
{
    public partial struct UpdateSystemsRunningData : ISystem
    {
        private uint m_PreviousFrameVersion;
        private uint m_CurrentFrameVersion;
        private int frequency;
        private float timer;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            frequency = 4;
            timer = 0;
        }

        public void OnUpdate(ref SystemState state)
        {
            m_CurrentFrameVersion = state.EntityManager.GlobalSystemVersion - m_PreviousFrameVersion;
            m_PreviousFrameVersion = state.EntityManager.GlobalSystemVersion;
            timer += SystemAPI.Time.DeltaTime;
            if (timer < (1.0 / frequency)) return;
            timer = 0;
            var systemCountUI = GameObject.FindGameObjectWithTag("SystemCount");
            if (systemCountUI == null) return;
            var tmp = systemCountUI.GetComponent<TextMeshProUGUI>();
            tmp.text = "System: " + m_CurrentFrameVersion;
        }
    }
}