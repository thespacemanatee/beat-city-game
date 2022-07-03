using UnityEditor;

namespace MoreMountains.Tools
{
    [CustomEditor(typeof(MMHealthBar), true)]
    /// <summary>
    /// Custom editor for health bars (mostly a switch for prefab based / drawn bars
    /// </summary>
    public class HealthBarEditor : Editor
    {
        public MMHealthBar HealthBarTarget => (MMHealthBar)target;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (HealthBarTarget.HealthBarType == MMHealthBar.HealthBarTypes.Prefab)
                DrawPropertiesExcluding(serializedObject, "Size", "BackgroundPadding", "SortingLayerName",
                    "InitialRotationAngles", "ForegroundColor", "DelayedColor", "BorderColor", "BackgroundColor",
                    "Delay", "LerpFrontBar", "LerpFrontBarSpeed", "LerpDelayedBar", "LerpDelayedBarSpeed",
                    "BumpScaleOnChange", "BumpDuration", "BumpAnimationCurve");

            if (HealthBarTarget.HealthBarType == MMHealthBar.HealthBarTypes.Drawn)
                DrawPropertiesExcluding(serializedObject, "HealthBarPrefab");

            serializedObject.ApplyModifiedProperties();
        }
    }
}