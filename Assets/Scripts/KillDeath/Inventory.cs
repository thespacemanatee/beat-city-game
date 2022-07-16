using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class Inventory<T> : ScriptableObject
{
    public bool gameStarted;
    public List<T> Items = new();

    public void Setup(int size)
    {
        for (var i = 0; i < size; i++) Items.Add(default);
    }

    public void Clear()
    {
        Items = new List<T>();
        gameStarted = false;
    }

    public void Add(T thing, int index)
    {
        if (index < Items.Count)
            Items[index] = thing;
    }

    public void Remove(int index)
    {
        if (index < Items.Count)
            Items[index] = default;
    }

    public T Get(int index)
    {
        if (index < Items.Count)
            return Items[index];
        return default;
    }
}