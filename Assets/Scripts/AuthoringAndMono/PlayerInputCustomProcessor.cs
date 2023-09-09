using UnityEngine.InputSystem;
using UnityEngine;
using UnityEditor;


#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class MousePositionToVector2Direction : InputProcessor<Vector2>
    {
#if UNITY_EDITOR
        static MousePositionToVector2Direction()
        {
            Initialize();
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            InputSystem.RegisterProcessor<MousePositionToVector2Direction>();
        }

        public override Vector2 Process(Vector2 value, InputControl control)
        {
            var offset = new Vector2(Screen.width, Screen.height) / 2;
            return (value - offset).normalized;
        }
    }