using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hammerScript : MonoBehaviour
{
    

    public Transform target;

    public LayerMask WhatIsTarget;

    public float fireRate, currentAttack;
    public float attackRange, stunRange;
    public bool attacking, toStun;


    


    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("dude") && attacking == false)
        {
            
            Hit();
            other.gameObject.GetComponent<enmy_scrip>().damaged(5);
            print("damage to deal");
            

        }
    }

     void ResetAttack()
    {
        attacking = false;                        //allows a delay between attacks

    }


    public void Hit()
    {
        toStun = true;
        attacking = true;
        Invoke(nameof(ResetAttack),fireRate);

    }

}