using UnityEngine;
using UnityEditor;
using Unity.Entities;
using ProjectDawn.Navigation;

namespace ProjectDawn.Navigation.Hybrid.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AgentColliderAuthoring))]
    class AgentColliderEditor : UnityEditor.Editor
    {
        static class Styles
        {
        }

        public override void OnInspectorGUI()
        {
        }
        
        void OnEnable()
        {
        }
    }
}
