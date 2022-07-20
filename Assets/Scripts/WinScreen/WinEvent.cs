using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WinEvent", menuName = "ScriptableObjects/WinEvent", order = 3)]
public class WinEvent : ScriptableObject
{
    private readonly List<WinEventListener> eventListeners = new();

    public void Raise(string W)
    {
        Debug.Log(eventListeners.Count);
        for (var i = eventListeners.Count - 1; i >= 0; i--)
            eventListeners[i].OnEventRaised(W);
    }

    public void RegisterListener(WinEventListener listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(WinEventListener listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}