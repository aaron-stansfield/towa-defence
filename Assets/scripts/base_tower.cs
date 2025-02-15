using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

public class Tower : MonoBehaviour
{
    private Transform target;
    private GameObject parentObject; // this gameObject's parent
    private GameObject turret; // this gameObject
    public GameObject upgradeMenu; // upgrade menu canvas
    private GameObject mouseColliderObject; // collider used for mouse raycasts
    private cameraScript cameraScript; // reference to the camera's script
    private game_managie manager;
    private bool isActive; // true if the tower is currently highlighted
    private float lastFireTime; // time since last shit
    private bool explosionUpgrade;

    public TextMeshProUGUI fireModeText;
    private int upgrade1Tier = 0;
    public TextMeshProUGUI upgrade1Text;
    private int upgrade2Tier = 0;
    public TextMeshProUGUI upgrade2Text;
    private int upgrade3Tier = 0;
    public TextMeshProUGUI upgrade3Text;
    private int bulletHealth = 1;
    public int baseCost = 5;
    public GameObject shooterPart;

    public float attackRadius = 30f;
    public float projectileSpeed = 60f;
    public float fireRate = 1f; // Time in seconds between shots
    public GameObject Bullet;
    public GameObject BSpawn;
    public GameObject currentTargetedObject;
    public GameObject highlightRing; // gameObject under the tower that spawns when the tower is highlighted
    public List<GameObject> enemysInRange;

    public targetState currentTargetState;
    public enum targetState
    {
        first,
        last,
        strong,
        weak
    }

    
    void Start()
    {
        manager = GameObject.Find("game managie").GetComponent<game_managie>();
        cameraScript = GameObject.Find("Main Camera").GetComponent<cameraScript>();
        mouseColliderObject = transform.GetChild(0).gameObject;
        //upgradeMenu = transform.GetChild(1).gameObject;
        turret = this.gameObject;
        upgradeMenu.gameObject.SetActive(false);
        parentObject = transform.parent.gameObject;
        currentTargetState = targetState.first;
    }

    
    void Update()
    {
        try
        {
            // purging enemy list of destroyed/ null enemies
            foreach (GameObject dude in enemysInRange)
            {
                if (dude == null)
                {
                    enemysInRange.Remove(dude);
                }
            }
        }

        catch(InvalidOperationException)
        { 
        
        }

        if (enemysInRange.Count != 0)
        {
            if (currentTargetState == targetState.first)
            {
                try
                {
                    target = getFirstEnemy(enemysInRange, false).transform;
                }
                catch (MissingReferenceException)
                {
                
                }

            }
            else if (currentTargetState == targetState.last)
            {
                try
                {
                    target = getFirstEnemy(enemysInRange, true).transform;
                }
                catch (MissingReferenceException)
                {

                }
                
            }

            try
            {
                //fire shot
                if (Vector3.Distance(transform.position, target.position) <= attackRadius && Time.time >= lastFireTime + fireRate)
                {
                    //Vector3 interceptPoint = CalculateInterceptPoint();
                    //if (interceptPoint != Vector3.zero)
                    //{
                    FireProjectile(target.transform.position);
                    shooterPart.GetComponent<Animation>().Play();
                    lastFireTime = Time.time; // Update the last fire time
                    //}
                    parentObject.transform.LookAt(new Vector3(target.transform.position.x,0,target.transform.position.z), Vector3.up);
                }
            }
            catch (MissingReferenceException)
            {

            }
        }

        // if player clicks tower open upgrade menu
        if (Input.GetMouseButtonDown(0) && cameraScript.currentMouseState == cameraScript.mouseState.normal && mouseColliderObject == GetClickedObject() && !manager.anyUpgradeMenuOpen)
        {
            print("clicked/touched!");
            upgradeMenu.gameObject.SetActive(true);
            highlightRing.SetActive(true);
            isActive = true;
            manager.anyUpgradeMenuOpen = true;

            //if time stops add here
        }

        if (Input.GetMouseButton(1) && isActive)
        {
            close();
        }
    }

    GameObject GetClickedObject()
    {
        GameObject target = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
        foreach(RaycastHit hit in hits)
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



    Vector3 CalculateInterceptPoint()
    {
        Vector3 toTarget = new Vector3(target.position.x,0,target.position.z) - transform.position;
        Vector3 targetVelocity = target.GetComponent<Rigidbody>().velocity;

        float a = Vector3.Dot(targetVelocity, targetVelocity) - projectileSpeed * projectileSpeed;
        float b = 2 * Vector3.Dot(targetVelocity, toTarget);
        float c = Vector3.Dot(toTarget, toTarget);

        float discriminant = b * b - 4 * a * c;
        if (discriminant < 0)
        {
            // No valid interception
            return Vector3.zero;
        }

        float t1 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
        float t2 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);

        float interceptTime = Mathf.Min(t1, t2);
        return target.position /*+ targetVelocity * interceptTime*/;
    }

    void FireProjectile(Vector3 interceptPoint)
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Transform projectile = Instantiate(Bullet.transform, transform.position, Quaternion.identity);
        projectile.GetComponent<Bullet>().health = bulletHealth;
        projectile.GetComponent<Bullet>().explosive = explosionUpgrade;
        projectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
    }

    public void Upgrade1()
    {
        if (upgrade1Tier == 0 && manager.money >= 10)
        {
            manager.money -= 10;
            fireRate = fireRate / 1.5f;
            upgrade1Tier++;
            upgrade1Text.text = "Tier 2 - cost 25\n further increases firerate";
        }
        else if (upgrade1Tier == 1 && manager.money >= 25)
        {
            manager.money -= 25;
            fireRate = fireRate / 1.3f;
            bulletHealth++;
            upgrade1Tier++;
            upgrade1Text.text = "fully upgraded";
        }
    }

    public void Upgrade2()
    {
        if (upgrade2Tier == 0 && manager.money >= 20)
        {
            manager.money -= 20;
            explosionUpgrade = true;
            upgrade2Tier++;
            upgrade2Text.text = "fully upgraded";
        }
    }

    public void Upgrade3()
    {
        print("upgrade 3");
        //input whatever upgrade we want
    }

    public void FireMode()
    {
        int currentIndex = Array.IndexOf(Enum.GetValues(typeof(targetState)), currentTargetState);
        if (currentIndex < 3)
        {

            currentTargetState++;
            
        }
        else
        {
            currentTargetState = 0;
        }
        fireModeText.text = currentTargetState.ToString();
    }
    public void close()
    {
        isActive = false;
        manager.anyUpgradeMenuOpen = false;
        upgradeMenu.gameObject.SetActive(false);
        highlightRing.gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider other) // adds enemy to enemylist
    {
        if (other.CompareTag("dude"))
        {
            enemysInRange.Add(other.gameObject);
        }
    }

    public void OnTriggerExit(Collider other) // removes enemy from enemylist
    {
        if(enemysInRange.Contains(other.gameObject))
        {
            enemysInRange.Remove(other.gameObject);
        }
    }

    GameObject getFirstEnemy(List<GameObject> wap, bool swap) // if swap = true then function returns last enemy instead
    {
        GameObject firstEnemy = wap[0];
        foreach (GameObject enemy in wap)
        {
            try
            {
                if (int.Parse(enemy.name) < int.Parse(firstEnemy.name) && !swap)
                {
                    firstEnemy = enemy;
                }

                else if (int.Parse(enemy.name) > int.Parse(firstEnemy.name) && swap)
                {
                    firstEnemy = enemy;
                }

            }
            catch (FormatException)
            {

            }
            catch (MissingReferenceException)
            {
            }
        }

        return firstEnemy;
    }
}
