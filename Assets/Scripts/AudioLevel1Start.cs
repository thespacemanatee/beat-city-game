using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLevel1Start : MonoBehaviour
{
    public AudioSource audioData;
    
    // Start is called before the first frame update
    void Start()
    {
        audioData.Play();
        audioData.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameObject.Find("StartUI")) {
            audioData.UnPause();
        }
    }
}
