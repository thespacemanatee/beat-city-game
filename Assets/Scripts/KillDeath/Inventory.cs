using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class Inventory<T> : ScriptableObject
{
    public bool gameStarted = false;
    public List<T> Items = new List<T>();

    public void Setup(int size){
        for (int i = 0; i < size; i++){
            Items.Add(default(T));
        }
    }

    public void Clear(){
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
            Items[index] = default(T);
    }

    public T Get(int index){
        if (index < Items.Count){
            return Items[index];
        }
        else return default(T);
    }
}