using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
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
        Debug.Log("Listen");
        Debug.Log(W);
        Response.Invoke(W);
    }
}