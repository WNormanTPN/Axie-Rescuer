using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using UnityEngine.Rendering;

namespace AxieRescuer
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(FireSystem))]
    public partial class DrawTrajectorySystem : SystemBase
    {
        private List<LineRenderer> lineList;
        protected override void OnCreate()
        {
            lineList = new List<LineRenderer>();
            RequireForUpdate<Trajectory>();
        }

        protected override void OnUpdate()
        {
            var trajectories = SystemAPI.GetSingletonBuffer<Trajectory>();
            for(int i = 0; i < trajectories.Length; i++)
            {
                DrawLine(trajectories[i].Start, trajectories[i].End, trajectories[i].Width, Color.yellow, trajectories[i].ShowTime);
            }
            trajectories.Clear();
        }

        void DrawLine(Vector3 start, Vector3 end, float width, Color color, float duration = 0.2f)
        {
            GameObject myLine = new GameObject();
            myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            lr.material.color = Color.yellow;
            lr.startWidth = width;
            lr.endWidth = width;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            GameObject.Destroy(myLine, duration);
        }
    }
}