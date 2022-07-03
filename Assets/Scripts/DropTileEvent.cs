using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DropTileEvent", menuName = "ScriptableObjects/DropTileEvent", order = 2)]
public class DropTileEvent : ScriptableObject
{
    private readonly List<DropTileEventListener> eventListeners =
        new List<DropTileEventListener>();

    public void Raise(List<int> indexes)
    {
        // Debug.Log("Raising power up collected");
        for (int i = eventListeners.Count - 1; i >= 0; i--)
            eventListeners[i].OnEventRaised(indexes);
    }

    public void RegisterListener(DropTileEventListener listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(DropTileEventListener listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}