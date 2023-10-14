using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
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
        TargetObjects = new NativeList<LocalTransform>(Allocator.Persistent);
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (TargetObjects.Length == 0) return;
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        var playerTransform = player.transform;
        var targetObject = TargetObjects[0];
        var nearestDis = math.distance(TargetObjects[0].Position, playerTransform.position);
        for(int i = 0; i < TargetObjects.Length; i++)
        {
            var dis = math.distance(TargetObjects[i].Position, playerTransform.position);
            if(dis < nearestDis)
            {
                nearestDis = dis;
                targetObject = TargetObjects[i];
            }
        }
        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(targetObject.Position);
        if (cutoutPos.x < 0 || cutoutPos.x > 1 || cutoutPos.y < 0 || cutoutPos.y > 1) return;
        cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 offset = (Vector3)targetObject.Position - transform.position;
        RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, wallMask);

        for (int i = 0; i < hitObjects.Length; ++i)
        {
            Material[] materials = hitObjects[i].transform.GetComponent<Renderer>().materials;

            for (int m = 0; m < materials.Length; ++m)
            {
                materials[m].SetVector("_CutoutPos", cutoutPos);
                materials[m].SetFloat("_CutoutSize", 0.3f);
                materials[m].SetFloat("_FalloffSize", 0.2f);
            }
        }
    }

    private void OnDestroy()
    {
        TargetObjects.Dispose();
    }
}
