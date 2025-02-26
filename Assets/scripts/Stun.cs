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
        
    }


}
