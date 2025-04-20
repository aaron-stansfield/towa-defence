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

    public void OnClick()
    {
        AudioPlay();
    }

    public void AudioPlay()
    {
       if (audio != null)
        {
            audio.Play();
        }
    
    }

}
