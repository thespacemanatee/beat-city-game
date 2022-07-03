using MoreMountains.Tools;
using UnityEditor;
using UnityEngine;

namespace MoreMountains.InventoryEngine
{
	/// <summary>
	///     Adds a dedicated InventoryEngine menu into the top bar More Mountains entry
	/// </summary>
	public static class InventoryEngineMenu
    {
        private const string _saveFolderName = "InventoryEngine";

        [MenuItem("Tools/More Mountains/Reset all saved inventories", false, 31)]
        /// <summary>
        /// Adds a menu item to reset all saved inventories directly from Unity. 
        /// This will remove the whole MMData/InventoryEngine folder, use it with caution.
        /// </summary>
        private static void ResetAllSavedInventories()
        {
            MMSaveLoadManager.DeleteSaveFolder(_saveFolderName);
            Debug.LogFormat("Inventories Save Files Reset");
        }
    }
}