using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinAudio : MonoBehaviour
{
    public GameObject WinScreen1;
    public GameObject WinScreen2;
    public GameObject WinScreen3;
    public GameObject WinScreen4;
    AudioSource audioData;

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
        if (WinScreen1.active || WinScreen2.active || WinScreen3.active || WinScreen4.active ) {
            audioData.UnPause();
        }
    }
}
