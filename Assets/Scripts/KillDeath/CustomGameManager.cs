using UnityEngine;
using UnityEngine.Events;

public class CustomGameManager : MonoBehaviour
{
    public UnityEvent onApplicationExit;

    private void OnApplicationQuit()
    {
        onApplicationExit.Invoke();
    }
}