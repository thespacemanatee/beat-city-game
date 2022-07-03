using UnityEditor;

namespace MoreMountains.Tools
{
    [CustomEditor(typeof(MMRadioSignal), true)]
    [CanEditMultipleObjects]
    public class MMRadioSignalEditor : Editor
    {
        protected SerializedProperty _currentLevel;

        protected SerializedProperty _duration;

        protected float _inspectorWidth;
        protected MMRadioSignal _radioSignal;

        protected virtual void OnEnable()
        {
            _radioSignal = target as MMRadioSignal;
            _duration = serializedObject.FindProperty("Duration");
            _currentLevel = serializedObject.FindProperty("CurrentLevel");
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _inspectorWidth = EditorGUIUtility.currentViewWidth - 24;

            DrawProperties();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawProperties()
        {
            DrawPropertiesExcluding(serializedObject, "AnimatedPreview", "CurrentLevel");
        }
    }
}