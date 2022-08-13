using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

[CreateAssetMenu(fileName = "IntVector3DictVariable", menuName = "ScriptableObjects/IntVector3DictVariable", order = 1)]
public class IntVector3DictVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline] public string DeveloperDescription = "Dictionary with int key and Vector3 value";
#endif
    private readonly Random rand = new();

    public Dictionary<int, Vector3> Value { get; } = new();

    public void Reset()
    {
        Value.Clear();
    }

    public void AddItem(int index, Vector3 vector3)
    {
        Value.Add(index, vector3);
    }

    public void UpdateItem(int index, Vector3 newValue)
    {
        Value[index] = newValue;
    }

    public void RemoveItem(int index)
    {
        Value.Remove(index);
    }

    public Vector3 GetItem(int index)
    {
        return Value[index];
    }

    public Vector3 GetRandomItem()
    {
        var values = Value.Values.ToList();
        var size = Value.Count;
        return values[rand.Next(size)];
    }
}