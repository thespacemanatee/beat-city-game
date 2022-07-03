using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class CustomDropTileEvent : UnityEvent<List<int>>
{
}

public class DropTileEventListener : MonoBehaviour
{
    public DropTileEvent @event;
    public CustomDropTileEvent response;

    private void OnEnable()
    {
        @event.RegisterListener(this);
    }

    private void OnDisable()
    {
        @event.UnregisterListener(this);
    }

    public void OnEventRaised(List<int> indexes)
    {
        response.Invoke(indexes);
    }
}