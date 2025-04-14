using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Audio : MonoBehaviour
{
    [SerializeField]
    public AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        if (audio == null)
        {
            audio = GetComponent<AudioSource>();
        }
      
    }

    public void PlayAudio()
    {
        if (audio != null)
        {
            audio.Play();
        }
        else
        {
            Debug.LogError("no audio source here");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
