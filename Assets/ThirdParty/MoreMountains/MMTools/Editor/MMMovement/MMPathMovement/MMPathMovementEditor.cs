#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MoreMountains.Tools
{
	/// <summary>
	///     This class adds names for each LevelMapPathElement next to it on the scene view, for easier setup
	/// </summary>
	[CustomEditor(typeof(MMPathMovement), true)]
    [InitializeOnLoad]
    public class MMPathMovementEditor : Editor
    {
        public MMPathMovement pathMovementTarget => (MMPathMovement)target;

        /// <summary>
        ///     OnSceneGUI, draws repositionable handles at every point in the path, for easier setup
        /// </summary>
        protected virtual void OnSceneGUI()
        {
            Handles.color = Color.green;
            var t = target as MMPathMovement;

            if (t.GetOriginalTransformPositionStatus() == false) return;

            for (var i = 0; i < t.PathElements.Count; i++)
            {
                EditorGUI.BeginChangeCheck();

                var oldPoint = t.GetOriginalTransformPosition() + t.PathElements[i].PathElementPosition;
                var style = new GUIStyle();

                // draws the path item number
                style.normal.textColor = Color.yellow;
                Handles.Label(
                    t.GetOriginalTransformPosition() + t.PathElements[i].PathElementPosition + Vector3.down * 0.4f +
                    Vector3.right * 0.4f, "" + i, style);

                // draws a movable handle
                var fmh_65_57_637918038871884790 = Quaternion.identity;
                var newPoint = Handles.FreeMoveHandle(oldPoint, .5f, new Vector3(.25f, .25f, .25f),
                    Handles.CircleHandleCap);

                // records changes
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Free Move Handle");
                    t.PathElements[i].PathElementPosition = newPoint - t.GetOriginalTransformPosition();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (pathMovementTarget.AccelerationType == MMPathMovement.PossibleAccelerationType.AnimationCurve)
                DrawDefaultInspector();
            else
                DrawPropertiesExcluding(serializedObject, "Acceleration");

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif