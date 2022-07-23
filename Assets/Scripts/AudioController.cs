using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    AudioSource audioData;
    private bool pause = false;
    
    // Start is called before the first frame update
    void Start()
    {
        audioData = GetComponent<AudioSource>();
        audioData.Play();
        audioData.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameObject.Find("StartUI")) {
            audioData.UnPause();
        }
        if (pause) {
            audioData.Pause();
        }
    }

    public void stopMusic()
    {
        Debug.Log("stopMusic called");
        pause = true;
    }
}
