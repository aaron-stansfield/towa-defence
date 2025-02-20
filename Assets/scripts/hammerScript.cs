using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hammerScript : MonoBehaviour
{
    public enmy_scrip enemy;

    public Transform target;

    public LayerMask WhatIsTarget;

    public float fireRate, currentAttack;
    public float attackRange, stunRange;
    public bool attacking, toStun;





    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("dude") && attacking == false)
        {
            currentAttack = 0;
            Hit();
            other.gameObject.GetComponent<enmy_scrip>().damaged(1);

        }
    }

    public void Hit()
    {

        attacking = true;
        currentAttack = Time.deltaTime;

        if (currentAttack >= fireRate)
        {
            attacking = false;
            currentAttack = 0;
        }

    }

    public void Stun()
    {
        toStun = true;
    }
}