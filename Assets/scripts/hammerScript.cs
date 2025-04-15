using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;

public class hammerScript : MonoBehaviour
{
    

    public Transform target;
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
    public GameObject Tier1Model;
    public GameObject Tier2Model;
    public GameObject Tier3Model;

    //upgrade
    private int upgrade1Tier = 0;
    public TextMeshProUGUI upgrade1Text;
    private int upgrade2Tier = 0;
    public TextMeshProUGUI upgrade2Text;
    private int upgrade3Tier = 0;
    public TextMeshProUGUI upgrade3Text;
    public bool attackKnockBack;
    public GameObject upgradeAnim;


    private void Start()
    {
        manager = GameObject.Find("game managie").GetComponent<game_managie>();
        cameraScript = Camera.main.GetComponent<cameraScript>();
        attackRange = 10;
        print("clicked/touched!");
        upgradeMenu.gameObject.SetActive(true);
        highlightRing.SetActive(true);
        isActive = true;
        manager.anyUpgradeMenuOpen = true;
    }


    private void Update()
    {

        if ((Input.GetMouseButton(1) || Input.GetMouseButtonDown(0)) && (isActive || manager.isProperPaused) && !isPointerOverUIObject())
        {
            close();
            if (cameraScript.movingArcerTarget)
            {
                cameraScript.changeMouseState();
                cameraScript.movingArcerTarget = false;
            }
        }

        if (Input.GetMouseButtonDown(0) && cameraScript.currentMouseState == cameraScript.mouseState.normal && mouseColliderObject == GetClickedObject() && !manager.anyUpgradeMenuOpen)
        {
            print("clicked/touched!");
            upgradeMenu.gameObject.SetActive(true);
            highlightRing.SetActive(true);
            isActive = true;
            manager.anyUpgradeMenuOpen = true;

            //if time stops add here
        }

    }

    private void FixedUpdate()
    {
        Collider[] potentialDudes = Physics.OverlapSphere(this.transform.position, attackRange, WhatIsTarget);

        if (attacking == false && potentialDudes.Length > 0)
        {

            attacking = true;
            StartCoroutine(attack());

        }
    }

    //public void OnTriggerStay(Collider other)
    //{

        
    //}

     void ResetAttack()
    {
        attacking = false;                        //allows a delay between attacks
    }


    void attackCall()
    {
        StartCoroutine(attack());
    }

    IEnumerator attack()
    {


        switch (upgrade1Tier)
        {
            case 0:
                Tier1Model.GetComponent<Animator>().SetTrigger("Hit");
                break;

            case 1 :
                Tier2Model.GetComponent<Animator>().SetTrigger("Hit");
                break;
            case 2 :
                Tier2Model.GetComponent<Animator>().SetTrigger("Hit");
                break;
            case >= 2:
                Tier3Model.GetComponent<Animator>().SetTrigger("Hit");
                break;
        }
        

        yield return new WaitForSeconds(0.2f);

        enemyCounter = 0;
        Collider[] potentialDudes = Physics.OverlapSphere(this.transform.position, attackRange, WhatIsTarget);
        foreach (Collider col in potentialDudes)
        {

            if (col.CompareTag("dude") && enemyCounter < 5)
            {
                if (attackKnockBack == true)
                {
                    col.GetComponent<enmy_scrip>().KnockBack();
                }
                else
                {
                    col.GetComponent<enmy_scrip>().Stun();
                }

                col.gameObject.GetComponent<enmy_scrip>().damaged(0);
                print("damage to deal");

                enemyCounter++;


            }

        }
        /*if (enemyCounter > 0)
        {
            Hit();
        }*/
        Invoke(nameof(ResetAttack), fireRate);
    }

    void Hit()
    {
        
        
        

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
            attackRange += 2;           //can change for balance
            highlightRing.transform.localScale = new Vector3(highlightRing.transform.localScale.x + (highlightRing.transform.localScale.x / 5), 0.37f,highlightRing.transform.localScale.z + (highlightRing.transform.localScale.z / 5));
            upgradeAnim.GetComponent<Animator>().SetTrigger("Start");
            Tier1Model.SetActive(false);
            Tier2Model.SetActive(true);
            upgrade1Tier++;
            upgrade1Text.text = "Tier 2 - cost 250\n further increases range";
        }
        else if (upgrade1Tier == 1 && manager.money >= 250)
        {
            manager.money -= 250;
            attackRange += 2;
            highlightRing.transform.localScale = new Vector3(highlightRing.transform.localScale.x + (highlightRing.transform.localScale.x / 6), 0.37f, highlightRing.transform.localScale.z + (highlightRing.transform.localScale.z / 6));
            upgradeAnim.GetComponent<Animator>().SetTrigger("Start");
            upgrade1Tier++;
            upgrade1Text.text = "Tier 3 - cost 400 \n Add knockback";
        }
        else if (upgrade1Tier == 2 && manager.money >= 400)
        {
            manager.money -= 400;
            attackKnockBack = true;
            upgradeAnim.GetComponent<Animator>().SetTrigger("Start");
            Tier2Model.SetActive(false);
            Tier3Model.SetActive(true);
            upgrade1Tier++;
            upgrade1Text.text = "fully upgraded \n :)";
        }

    }

    
            
}