using UnityEditor;
using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     This class lets you clean all missing scripts on a selection of gameobjects
    /// </summary>
    public class MMCleanupMissingScripts : MonoBehaviour
    {
        /// <summary>
        ///     Processes the cleaning of gameobjects for all missing scripts on them
        /// </summary>
        [MenuItem("Tools/More Mountains/Cleanup missing scripts on selected GameObjects", false, 504)]
        protected static void CleanupMissingScripts()
        {
            var collectedDeepHierarchy = EditorUtility.CollectDeepHierarchy(Selection.gameObjects);
            var removedComponentsCounter = 0;
            var gameobjectsAffectedCounter = 0;
            foreach (var targetObject in collectedDeepHierarchy)
                if (targetObject is GameObject gameObject)
                {
                    var amountOfMissingScripts = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gameObject);
                    if (amountOfMissingScripts > 0)
                    {
                        Undo.RegisterCompleteObjectUndo(gameObject, "Removing missing scripts");
                        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
                        removedComponentsCounter += amountOfMissingScripts;
                        gameobjectsAffectedCounter++;
                    }
                }

            Debug.Log("[MMCleanupMissingScripts] Removed " + removedComponentsCounter + " missing scripts from " +
                      gameobjectsAffectedCounter + " GameObjects");
        }
    }
}