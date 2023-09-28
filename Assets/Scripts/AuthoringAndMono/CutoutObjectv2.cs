using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;

public class CutoutObjectv2 : MonoBehaviour
{
    public static NativeList<LocalTransform> TargetObjects;

    [SerializeField]
    private LayerMask wallMask;

    private Camera mainCamera;

    private void Awake()
    {
        TargetObjects = new NativeList<LocalTransform>();
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        foreach (var targetObject in TargetObjects)
        {
            Vector2 cutoutPos = mainCamera.WorldToViewportPoint(targetObject.Position);
            cutoutPos.y /= (Screen.width / Screen.height);

            Vector3 offset = (Vector3)targetObject.Position - transform.position;
            RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, wallMask);

            for (int i = 0; i < hitObjects.Length; ++i)
            {
                Material[] materials = hitObjects[i].transform.GetComponent<Renderer>().materials;

                for (int m = 0; m < materials.Length; ++m)
                {
                    materials[m].SetVector("_CutoutPos", cutoutPos);
                    materials[m].SetFloat("_CutoutSize", 0.1f);
                    materials[m].SetFloat("_FalloffSize", 0.05f);
                }
            }
        }
    }
}
