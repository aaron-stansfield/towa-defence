using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class hammerScript : MonoBehaviour
{
    

    public Transform target;
    public GameObject wackFX;
    public GameObject hammerFX;
    [SerializeField] GameObject highlightRing;
    [SerializeField] GameObject upgradeMenu;
    public LayerMask WhatIsTarget;
    //[SerializeField] LayerMask enemyLayer;
    private cameraScript cameraScript;
    public float fireRate, currentAttack;
    public float attackRange, stunRange;
    public bool attacking, toStun, inStunZone;
    [SerializeField] GameObject mouseColliderObject;
    private bool isActive;
    private game_managie manager;
    [SerializeField] int enemyCounter;

    //upgrade
    private int upgrade1Tier = 0;
    public TextMeshProUGUI upgrade1Text;
    private int upgrade2Tier = 0;
    public TextMeshProUGUI upgrade2Text;
    private int upgrade3Tier = 0;
    public TextMeshProUGUI upgrade3Text;
    public bool attackKnockBack;


    private void Start()
    {
        manager = GameObject.Find("game managie").GetComponent<game_managie>();
        cameraScript = Camera.main.GetComponent<cameraScript>();
        attackRange = 10;
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && cameraScript.currentMouseState == cameraScript.mouseState.normal && mouseColliderObject == GetClickedObject() && !manager.anyUpgradeMenuOpen)
        {
            print("clicked/touched!");
            upgradeMenu.gameObject.SetActive(true);
            highlightRing.SetActive(true);
            isActive = true;
            manager.anyUpgradeMenuOpen = true;

            //if time stops add here
        }

        if (Input.GetMouseButton(1) && isActive || manager.isProperPaused)
        {
            close();
            if (cameraScript.movingArcerTarget)
            {
                cameraScript.changeMouseState();
                cameraScript.movingArcerTarget = false;
            }
        }

        if (Input.GetMouseButton(1) && isActive || manager.isProperPaused)
        {
            close();
            if (cameraScript.movingArcerTarget)
            {
                cameraScript.changeMouseState();
                cameraScript.movingArcerTarget = false;
            }
        }
    }

    private void FixedUpdate()
    {
        
        if (Physics.OverlapSphere(this.transform.position, attackRange, WhatIsTarget) != null && attacking == false)
        {

            hammerFX.GetComponent<Animation>().Play();
            StartCoroutine(attack());

        }
    }

    //public void OnTriggerStay(Collider other)
    //{

        
    //}

     void ResetAttack()
    {
        attacking = false;                        //allows a delay between attacks
        wackFX.SetActive(false);
    }


    IEnumerator attack()
    {
        yield return new WaitForSeconds(0.4f);
        enemyCounter = 0;
        foreach (Collider col in Physics.OverlapSphere(this.transform.position, 15, WhatIsTarget))
        {

            if (col.CompareTag("dude") && enemyCounter < 11)
            {
                if (attackKnockBack == true)
                {
                    col.GetComponent<enmy_scrip>().KnockBack();
                }
                col.GetComponent<enmy_scrip>().Stun();
                col.gameObject.GetComponent<enmy_scrip>().damaged(0);
                print("damage to deal");

                enemyCounter++;

            }
        }
        Hit();
    }

    void Hit()
    {
        
        toStun = true;
        attacking = true;
        wackFX.SetActive(true);
        wackFX.GetComponent<Animator>().Play("wackerFXAnimation");
        
        Invoke(nameof(ResetAttack),fireRate);
        

    }


    GameObject GetClickedObject()
    {
        GameObject target = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
        foreach (RaycastHit hit in hits)
        {
            Debug.Log(hit.collider.gameObject.name);
            if (!isPointerOverUIObject() && hit.collider.CompareTag("towerCollider"))
            {
                target = hit.collider.gameObject;
                return target;
            }
        }
        //might need to add if (!isPointerOverUIObject()) {}
        return target;
    }


    private bool isPointerOverUIObject()  //avoids the click interacting with ui
    {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);
        return results.Count > 0;
    }

    public void close()
    {
        isActive = false;
        manager.anyUpgradeMenuOpen = false;
        upgradeMenu.gameObject.SetActive(false);
        highlightRing.gameObject.SetActive(false);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawSphere(this.transform.position, 20);
    //}

    public void Upgrade1()
    {
        if (upgrade1Tier == 0 && manager.money >= 100)
            {
                manager.money -= 100;
                attackRange += 2;           //can change for ballance
                upgrade1Tier++;
                upgrade1Text.text = "Tier 2 - cost 250\n further increases range";
            }
            else if(upgrade1Tier == 1 && manager.money >= 250)
            {
                manager.money -= 250;
                attackRange += 2;
                upgrade1Tier++;
                upgrade1Text.text = "fully upgraded \n :)";
            }
        
    }

    public void Upgrade2()
    {

        if (upgrade2Tier == 0 && manager.money >= 200)
            {
                manager.money -= 200;
                attackKnockBack = true;
                upgrade2Tier++;
                upgrade2Text.text = "fully upgraded \n :)";
            }
    }
            
}