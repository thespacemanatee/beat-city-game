using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class CustomDropTileEvent : UnityEvent<List<int>>
{
}

public class DropTileEventListener : MonoBehaviour
{
    public DropTileEvent Event;
    public CustomDropTileEvent Response;
    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(List<int> indexes)
    {
        Response.Invoke(indexes);
    }
}