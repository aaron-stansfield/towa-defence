using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hammerScript : MonoBehaviour
{
    

    public Transform target;
    public GameObject wackFX;
    public LayerMask WhatIsTarget;

    public float fireRate, currentAttack;
    public float attackRange, stunRange;
    public bool attacking, toStun, inStunZone;


    


    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("dude"))
        {
            inStunZone = true;
        }

        if (other.CompareTag("dude") && attacking == false)
        {
            
            Hit();
            other.gameObject.GetComponent<enmy_scrip>().damaged(10);
            print("damage to deal");
            

        }
    }

     void ResetAttack()
    {
        attacking = false;                        //allows a delay between attacks
        wackFX.SetActive(false);
    }


    public void Hit()
    {
        toStun = true;
        attacking = true;
        wackFX.SetActive(true);
        wackFX.GetComponent<Animator>().Play("wackerFXAnimation");
        Invoke(nameof(ResetAttack),fireRate);

    }

}