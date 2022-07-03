using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DropTileEvent", menuName = "ScriptableObjects/DropTileEvent", order = 2)]
public class DropTileEvent : ScriptableObject
{
    private readonly List<DropTileEventListener> _eventListeners = new();

    public void Raise(List<int> indexes)
    {
        // Debug.Log("Raising power up collected");
        for (var i = _eventListeners.Count - 1; i >= 0; i--)
            _eventListeners[i].OnEventRaised(indexes);
    }

    public void RegisterListener(DropTileEventListener listener)
    {
        if (!_eventListeners.Contains(listener))
            _eventListeners.Add(listener);
    }

    public void UnregisterListener(DropTileEventListener listener)
    {
        if (_eventListeners.Contains(listener))
            _eventListeners.Remove(listener);
    }
}