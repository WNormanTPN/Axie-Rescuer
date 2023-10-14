//using Unity.Mathematics;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class CursorManageAuthoring : MonoBehaviour
//{
//    private RectTransform NewCursor;

//    void Start()
//    {
//        //Cursor.visible = false;
//        NewCursor = gameObject.transform.Find("Cursor").GetComponent<RectTransform>();
//    }

//    void Update()
//    {
//        var player = GameObject.FindGameObjectWithTag("Player");
//        if (player == null) return;
//        var playerAnimator = player.GetComponent<Animator>();
//        if (playerAnimator == null) return;
//        var playerViewPos = Camera.main.WorldToViewportPoint(player.transform.position);
//        var offsetAngle = -Camera.main.transform.rotation.y;
//        var w = Screen.width;
//        var h = Screen.height;
//        var cursorPos = Mouse.current.position.value;
//        cursorPos.x -= w / 2;
//        cursorPos.y -= h / 2;
//        var dis = Vector3.Distance(playerViewPos, cursorPos);
//        var direct = playerAnimator.transform.forward;
//        var x = direct.x * math.cos(offsetAngle) + direct.z * math.sin(offsetAngle);
//        var z = -direct.x * math.sin(offsetAngle) + direct.z * math.cos(offsetAngle);
//        direct.x = x;
//        direct.z = z;
//        NewCursor.anchoredPosition = new Vector3(direct.x, direct.z, 0) * dis;
//    }
//}
