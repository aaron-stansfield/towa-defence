using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
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
    public bool arcer; // if this tower shoots in an arc
    public float launchSpeed;
    public GameObject arcerTargetObj;
    public bool slowOnHit;
    private bool extraSlowChance;
    private int explosionDamage;
    [SerializeField] bool gumballer;
    private float sauceLifeSpan = 1f;
    public Mesh hotDogProjectileMesh;


    public TextMeshProUGUI fireModeText;
    private int upgrade1Tier = 0;
    public TextMeshProUGUI upgrade1Text;
    private int upgrade2Tier = 0;
    public TextMeshProUGUI upgrade2Text;
    private int upgrade3Tier = 0;
    public TextMeshProUGUI upgrade3Text;
    private int bulletHealth;
    public GameObject shooterPart; // part of gumballer that animates when it shoots

    public float attackRadius;
    public float projectileSpeed;
    public float fireRate = 1f; // Time in seconds between shots
    public GameObject Bullet;
    public GameObject BSpawn;
    public GameObject currentTargetedObject;
    public GameObject highlightRing; // gameObject under the tower that spawns when the tower is highlighted
    public List<GameObject> enemysInRange;
    private GameObject placeAnim;

    public targetState currentTargetState;
    public enum targetState
    {
        first,
        last,
        strong,
        weak,
        user_set
    }

    
    void Start()
    {
        //placeAnim = GameObject.Find("placeAnim").gameObject;
        manager = GameObject.Find("game managie").GetComponent<game_managie>();
        cameraScript = GameObject.Find("Main Camera").GetComponent<cameraScript>();
        mouseColliderObject = transform.GetChild(0).gameObject;
        //upgradeMenu = transform.GetChild(1).gameObject;
        turret = this.gameObject;
        upgradeMenu.gameObject.SetActive(false);
        parentObject = transform.parent.gameObject;
        if (gumballer)
        {
            explosionDamage = 4;
            bulletHealth = 2;
        }
        else if (arcer)
        {
            explosionDamage = 7;
            bulletHealth = 1;
        }
    }

    
    void Update()
    {

        // if player clicks tower open upgrade menu
        if (Input.GetMouseButtonDown(0) && cameraScript.currentMouseState == cameraScript.mouseState.normal && mouseColliderObject == GetClickedObject() && !manager.anyUpgradeMenuOpen)
        {
            print("clicked/touched!");
            upgradeMenu.gameObject.SetActive(true);
            highlightRing.SetActive(true);
            isActive = true;
            manager.anyUpgradeMenuOpen = true;
            if (arcer)
            {
                arcerTargetObj.SetActive(true);
            }


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

    }

    public void FixedUpdate()
    {
        // purging enemy list of destroyed/ null enemies
        try
        {
            foreach (GameObject dude in enemysInRange)
            {
                if (!dude.CompareTag("dude"))
                {
                    enemysInRange.Remove(dude);
                }
            }
        }
        catch (InvalidOperationException)
        {

        }

        //fire shot
        if (Time.time >= lastFireTime + fireRate)
        {
            //Vector3 interceptPoint = CalculateInterceptPoint();
            //if (interceptPoint != Vector3.zero){}
            if (currentTargetState == targetState.first && enemysInRange.Count != 0)
            {

                target = getFirstEnemy(enemysInRange, false).transform;


            }
            else if (currentTargetState == targetState.last && enemysInRange.Count != 0)
            {

                target = getFirstEnemy(enemysInRange, true).transform;

            }
            else if (currentTargetState == targetState.user_set)
            {
                target = arcerTargetObj.transform;
            }
            else
            {
                return;
            }

            if (target != null && Vector3.Distance(transform.position, target.position) <= attackRadius)
            {
                FireProjectile(target.transform.position);
                if (gumballer)
                {
                    shooterPart.GetComponent<Animation>().Play();
                }

                lastFireTime = Time.time; // Update the last fire time

                transform.LookAt(new Vector3(target.transform.position.x, (this.GetComponent<Renderer>().bounds.center.y), target.transform.position.z), Vector3.up);
            }
            
        }
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



    Vector3 CalculateInterceptPoint()
        {
            Vector3 toTarget = new Vector3(target.position.x, 0, target.position.z) - transform.position;
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
    // Calculate the direction from the current position to the target position, normalized to get a unit vector
    Vector3 direction = (target.transform.position - transform.position).normalized;

    // Instantiate a new projectile (Bullet) at the current position with no rotation
    Transform projectile = Instantiate(Bullet.transform, transform.position, Quaternion.identity);

    // Get the Bullet script component attached to the projectile
    Bullet bulletScript = projectile.GetComponent<Bullet>();

    // Set various properties of the bullet based on the current settings
    bulletScript.explosionDamage = explosionDamage;
    bulletScript.health = bulletHealth;
    bulletScript.slows = slowOnHit;
    bulletScript.extraSlowChance = extraSlowChance;
   
    // Check if the tower is a "gumballer"
    if (gumballer)
    {

        direction = (target.transform.position - transform.position).normalized;
        projectile = Instantiate(Bullet.transform, BSpawn.transform.position, Quaternion.identity);
        bulletScript = projectile.GetComponent<Bullet>();
        bulletScript.explosionDamage = explosionDamage;
        bulletScript.health = bulletHealth;
        bulletScript.slows = slowOnHit;
        bulletScript.extraSlowChance = extraSlowChance;
       
        if (gumballer)
        {
            projectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
            bulletScript.chanceExplosive = explosionUpgrade;

                bulletScript.bulletLifeTime = 2.4f;
            }
        else if(arcer)
        {

            projectile.GetComponent<MeshFilter>().mesh = hotDogProjectileMesh;
            bulletScript.sauceLifeSpan = sauceLifeSpan;
            bulletScript.slows = slowOnHit;
            bulletScript.extraSlowChance = extraSlowChance;
            projectile.GetComponent<Rigidbody>().useGravity = true;
            bulletScript.explosive = true;
            bulletScript.parabolic = true;
            bulletScript.bulletLifeTime = 10f;
            projectile.transform.rotation = this.transform.rotation;
            bulletScript.angle = target.position - this.transform.position;
            Vector3 force = (this.transform.position - target.transform.position).normalized;
            // from 5 - 90 to 3 - 16.5
            float targetDistance = Vector3.Distance(this.transform.position, target.transform.position);
            Debug.Log("losing my mind");
            launchSpeed = Remap(targetDistance, 5, 90, 5, 16.5f);
            launchSpeed = launchSpeed * 1.05f;
            projectile.GetComponent<Rigidbody>().AddForce(new Vector3(-force.x, 1.5f, -force.z) * launchSpeed, ForceMode.Impulse);
            }

        // Set the velocity of the projectile's Rigidbody to move it towards the target
        projectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
        
        // Set the explosive chance upgrade for the bullet
        bulletScript.chanceExplosive = explosionUpgrade;
    }
    // Check if the tower is an "arcer"
    else if(arcer)
    {
        // Set additional properties specific to "arcer" projectiles
        bulletScript.sauceLifeSpan = sauceLifeSpan;
        bulletScript.slows = slowOnHit;
        bulletScript.extraSlowChance = extraSlowChance;
        projectile.GetComponent<Rigidbody>().useGravity = true;
        bulletScript.explosive = true;
        bulletScript.parabolic = true;
        bulletScript.bulletLifeTime = 10f;

        // Calculate the force direction for the projectile
        Vector3 force = (this.transform.position - target.transform.position).normalized;
        
        // Calculate the distance to the target
        float targetDistance = Vector3.Distance(this.transform.position, target.transform.position);
        
        // Debug log to check the flow of execution
        Debug.Log("losing my mind");
        
        // Remap the target distance to an appropriate launch speed
        launchSpeed = Remap(targetDistance, 5, 90, 5, 16.5f);
        
        // Adjust the launch speed slightly
        launchSpeed = launchSpeed * 1.13f;

        // Apply force to the projectile's Rigidbody to launch it in a parabolic trajectory
        projectile.GetComponent<Rigidbody>().AddForce(new Vector3(-force.x, 1.5f, -force.z) * launchSpeed, ForceMode.Impulse);
    }
}


    float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (float)((value - from1) / (to1 - from1) * (to2 - from2) + from2);
    }

    void Trajectory(Vector3 end)
    {
        //Vector3 initialVelocity = new Vector3(;
    }

    public void Upgrade1()
    {
        if (gumballer)
        {
            // upgrades for the gumballer
            if (upgrade1Tier == 0 && manager.money >= 100)
            {
                manager.money -= 100;
                fireRate = fireRate / 1.5f;
                upgrade1Tier++;
                upgrade1Text.text = "Tier 2 - cost 250\n further increases firerate";
            }
            else if (upgrade1Tier == 1 && manager.money >= 250)
            {
                manager.money -= 250;
                fireRate = fireRate / 1.3f;
                bulletHealth++;
                upgrade1Tier++;
                upgrade1Text.text = "fully upgraded \n :)";
            }
        }

        else if(arcer)
        {
            // upgrades for the dogger
            if (upgrade1Tier == 0 && manager.money >= 100)
            {
                manager.money -= 100;
                fireRate = fireRate / 1.5f;
                explosionDamage = 7;
                upgrade1Tier++;
                upgrade1Text.text = "Tier 2 - cost 250\n further increases firerate";
            }
            else if(upgrade1Tier == 1 && manager.money >= 250)
            {
                manager.money -= 250;
                fireRate = fireRate / 1.5f;
                upgrade1Tier++;
                upgrade1Text.text = "fully upgraded \n :)";
            }
        }
        
    }

    public void Upgrade2()
    {

        // upgrades for the gumballer
        if (gumballer)
        {
            if (upgrade2Tier == 0 && manager.money >= 200)
            {
                manager.money -= 200;
                explosionUpgrade = true;
                upgrade2Tier++;
                upgrade2Text.text = "fully upgraded \n :)";
            }
        }
        
        else if (arcer)
        {
            if (upgrade2Tier == 0 && manager.money >= 200)
            {
                manager.money -= 200;
                sauceLifeSpan = 6;
                upgrade2Tier++;
                upgrade2Text.text = "fully upgraded \n :)";
            }
            //if (upgrade2Tier == 1 && manager.money >= 150)
        }
    }

    public void Upgrade3()
    {
        if (gumballer)
        {
            // upgrades for the gumballer
            if (upgrade3Tier == 0 && manager.money >= 150)
            {
                //manager.money -= 150;
                //slowOnHit = true;
                //upgrade3Tier++;
                //upgrade3Text.text = "tier 2 - cost 200 \n increase chance to slow enemies";
            }
            else if (upgrade3Tier == 1 && manager.money >= 250)
            {
                //manager.money -= 200;
                //extraSlowChance = true;
                //upgrade3Tier++;
                //upgrade3Text.text = "fully upgraded \n :)";
            }
        }
        
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
        if (arcer)
        {
            arcerTargetObj.SetActive(false);
        }   
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


    public void changeArcerTarget()
    {
        cameraScript.movingArcerTarget = true;
        cameraScript.towerGhost = arcerTargetObj;
        cameraScript.changeMouseState();
    }

    GameObject getFirstEnemy(List<GameObject> wap, bool swap) // if swap = true then function returns last enemy instead
    {
        GameObject firstEnemy = wap[0];
        NavMeshAgent enmyNav;
        foreach (GameObject enemy in wap)
        {
            enmyNav = enemy.GetComponent<NavMeshAgent>();
            if (int.Parse(enemy.name) < int.Parse(firstEnemy.name) && !swap)
            {
                firstEnemy = enemy;
            }

            else if (int.Parse(enemy.name) > int.Parse(firstEnemy.name) && swap)
            {
                firstEnemy = enemy;
            }

        }

        return firstEnemy;
    }   

    //public float GetPathRemainingDistance(NavMeshAgent navMeshAgent)
    //{
    //    float distance = 0.0f;
    //    for (int i = 0; i < navMeshAgent.path.corners.Length - 1; ++i)
    //    {
    //        distance += Vector3.Distance(navMeshAgent.path.corners[i - 1], navMeshAgent.path.corners[i]); ;
    //    }
    //    Debug.Log(distance);
    //    return distance;
    //}
}
