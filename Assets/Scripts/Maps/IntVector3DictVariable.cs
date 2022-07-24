using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IntVector3DictVariable", menuName = "ScriptableObjects/IntVector3DictVariable", order = 1)]
public class IntVector3DictVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline] public string DeveloperDescription = "Dictionary with int key and Vector3 value";
#endif

    public Dictionary<int, Vector3> Value { get; private set; } = new Dictionary<int, Vector3>();
    private System.Random rand = new System.Random();

    public void AddItem(int index, Vector3 vector3)
    {
        Value.Add(index, vector3);
    }

    public void UpdateItem(int index, Vector3 newValue)
    {
        Value[index] = newValue;
    }

    public void Reset()
    {
        Value.Clear();
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
        List<Vector3> values = Enumerable.ToList(Value.Values);
        int size = Value.Count;
        return values[rand.Next(size)];
    }
}
