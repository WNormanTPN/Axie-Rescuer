using UnityEngine;
using UnityEditor;
using Unity.Entities;
using ProjectDawn.Navigation;

namespace ProjectDawn.Navigation.Hybrid.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AgentAuthoring))]
    class AgentEditor : UnityEditor.Editor
    {
        static class Styles
        {
            public static readonly GUIContent MotionType = EditorGUIUtility.TrTextContent("Motion Type", "The type of agent motion.");
            public static readonly GUIContent Speed = EditorGUIUtility.TrTextContent("Speed", "Maximum movement speed when moving to destination.");
            public static readonly GUIContent Acceleration = EditorGUIUtility.TrTextContent("Acceleration", "The maximum acceleration of an agent as it follows a path, given in units / sec^2.");
            public static readonly GUIContent AngularSpeed = EditorGUIUtility.TrTextContent("Angular Speed", "Maximum turning speed in (deg/s) while following a path.");
            public static readonly GUIContent StoppingDistance = EditorGUIUtility.TrTextContent("Stopping Distance", "Stop within this distance from the target position.");
            public static readonly GUIContent AutoBreaking = EditorGUIUtility.TrTextContent("Auto Breaking", "Should the agent brake automatically to avoid overshooting the destination point?");
        }

        SerializedProperty m_MotionType;
        SerializedProperty m_Speed;
        SerializedProperty m_Acceleration;
        SerializedProperty m_AngularSpeed;
        SerializedProperty m_StoppingDistance;
        SerializedProperty m_AutoBreaking;

        AgentAuthoring Agent => target as AgentAuthoring;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUI.DisabledScope(Application.isPlaying))
                EditorGUILayout.PropertyField(m_MotionType, Styles.MotionType);

            if (m_MotionType.enumValueIndex == (int)AgentMotionType.Steering)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_Speed, Styles.Speed);
                EditorGUILayout.PropertyField(m_Acceleration, Styles.Acceleration);
                EditorGUILayout.PropertyField(m_AngularSpeed, Styles.AngularSpeed);
                EditorGUILayout.PropertyField(m_StoppingDistance, Styles.StoppingDistance);
                EditorGUILayout.PropertyField(m_AutoBreaking, Styles.AutoBreaking);
                EditorGUI.indentLevel--;
            }

            if (m_MotionType.enumValueIndex == (int) AgentMotionType.Static)
            {
                EditorGUILayout.HelpBox("Static is currently not supported use dynamic!", MessageType.Error);
            }

            if (!serializedObject.isEditingMultipleObjects)
            {
                if (target is AgentAuthoring auth && auth.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>() != null)
                    EditorGUILayout.HelpBox("This component does not work with NavMeshAgent!", MessageType.Error);
                if (Agent.GetComponent<AgentCylinderShapeAuthoring>() == null && Agent.GetComponent<AgentCircleShapeAuthoring>() == null)
                    EditorGUILayout.HelpBox("It is expected that agent will have Cylinder or Circle shape!", MessageType.Warning);
            }

            if (serializedObject.ApplyModifiedProperties())
            {
                // Update all agents entities steering
                foreach (var target in targets)
                {
                    var authoring = target as AgentAuthoring;
                    if (authoring.HasEntitySteering)
                        authoring.EntitySteering = authoring.DefaultSteering;
                }
            }
        }
        
        void OnEnable()
        {
            m_MotionType = serializedObject.FindProperty("MotionType");
            m_Speed = serializedObject.FindProperty("Speed");
            m_Acceleration = serializedObject.FindProperty("Acceleration");
            m_AngularSpeed = serializedObject.FindProperty("AngularSpeed");
            m_StoppingDistance = serializedObject.FindProperty("StoppingDistance");
            m_AutoBreaking = serializedObject.FindProperty("AutoBreaking");

            if (Application.isPlaying)
            {
                var world = World.DefaultGameObjectInjectionWorld;
                if (world == null)
                    return;
                var manager = world.EntityManager;
                if (!manager.HasComponent<DrawGizmos>(Agent.GetOrCreateEntity()))
                {
                    manager.AddComponent<DrawGizmos>(Agent.GetOrCreateEntity());
                }
            }
        }

        void OnDisable()
        {
            if (Application.isPlaying)
            {
                var world = World.DefaultGameObjectInjectionWorld;
                if (world == null)
                    return;
                var manager = world.EntityManager;
                if (manager.HasComponent<DrawGizmos>(Agent.GetOrCreateEntity()))
                {
                    manager.RemoveComponent<DrawGizmos>(Agent.GetOrCreateEntity());
                }
            }
        }

        void OnSceneGUI()
        {
            // Call OnSceneGUI only once
            if (target == Selection.activeObject)
                return;

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null)
                return;
            var gizmosSystem = world.Unmanaged.GetExistingUnmanagedSystem<GizmosSystem>();
            if (gizmosSystem == SystemHandle.Null)
                return;
            var gizmos = world.EntityManager.GetComponentData<GizmosSystem.Singleton>(gizmosSystem);
            gizmos.ExecuteCommandBuffers();
        }
    }
}
