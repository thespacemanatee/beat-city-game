using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class CustomKDEvent : UnityEvent<string, string>
{
}

public class KDEventListener : MonoBehaviour
{
    public KDEvent Event;
    public CustomKDEvent Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(string k, string d)
    {
        Response.Invoke(k, d);
    }
}