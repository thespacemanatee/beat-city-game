using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class CustomEvent : UnityEvent<string>
{
}

public class WinEventListener : MonoBehaviour
{
    public WinEvent Event;
    public CustomEvent Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(string W)
    {
        Response.Invoke(W);
    }
}