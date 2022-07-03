using UnityEngine;
using UnityEngine.Events;

public class CustomGameManager : MonoBehaviour
{
    public UnityEvent onApplicationExit;

	void OnApplicationQuit()
    {
        onApplicationExit.Invoke();
    }
}
