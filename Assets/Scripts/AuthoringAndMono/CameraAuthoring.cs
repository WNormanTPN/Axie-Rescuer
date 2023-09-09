using Unity.Entities;
using UnityEngine;

namespace AxieRescuer
{
    public class CameraAuthoring : MonoBehaviour
    {
        public Vector3 PositionOffset;
        public Vector3 InitialRotation;

        public class CameraBaker : Baker<CameraAuthoring>
        {
            public override void Bake(CameraAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new FollowOffset
                {
                    Position = authoring.PositionOffset,
                    Rotation = authoring.InitialRotation
                });
            }
        }
    }
}