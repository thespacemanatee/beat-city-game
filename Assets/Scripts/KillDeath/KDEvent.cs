using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KDEvent", menuName = "ScriptableObjects/KDEvent", order = 3)]
public class KDEvent : ScriptableObject
{
     private readonly List<KDEventListener> eventListeners = 
        new List<KDEventListener>();

    public void Raise(string  k, string d)
    {
        for(int i = eventListeners.Count -1; i >= 0; i--)
            eventListeners[i].OnEventRaised(k, d);
    }

    public void RegisterListener(KDEventListener listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(KDEventListener listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}
