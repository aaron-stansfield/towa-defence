using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stun : MonoBehaviour
{
    hammerScript hammerTowerScript;
    
    private void Awake()
    {
        hammerTowerScript = GameObject.FindGameObjectWithTag("hammer").GetComponent<hammerScript>();
    }
    


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("dude") && hammerTowerScript.toStun == true)
        {
            other.GetComponent<enmy_scrip>().Stun();

            Invoke(nameof(SetToFalse),1);
           
        }
    }

    private void SetToFalse()
    {
        hammerTowerScript.toStun = false;
    }
}
