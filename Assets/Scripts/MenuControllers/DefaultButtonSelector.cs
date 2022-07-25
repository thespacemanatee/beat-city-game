using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DefaultButtonSelector : MonoBehaviour
{
    public GameObject defaultButton;
    
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(defaultButton);    
    }

    void OnEnable() 
    {
        EventSystem.current.SetSelectedGameObject(defaultButton);
    }

}
