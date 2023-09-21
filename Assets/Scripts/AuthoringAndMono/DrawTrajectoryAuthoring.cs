using AxieRescuer;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DrawTrajectoryAuthoring : MonoBehaviour
{
    public class DrawTrajectoryBaker : Baker<DrawTrajectoryAuthoring>
    {
        public override void Bake(DrawTrajectoryAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddBuffer<Trajectory>(entity);
        }
    }
}
