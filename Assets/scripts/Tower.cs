using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class Tower : MonoBehaviour
{
    private Transform target;
    private bool ActiveTower;
    public float MaxShootTime;
    public GameObject Bullet;
    public GameObject BSpawn;
    public float attackRadius = 30f;
    public float projectileSpeed = 60f;
    public float fireRate = 1f; // Time in seconds between shots
    private float lastFireTime;
    public GameObject currentTargetedObject;
    public List<GameObject> enemysInRange;

    private GameObject parentObject;
    private GameObject turret;
    private GameObject upgradeMenu;

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
        //  StartCoroutine("ReachCheck");
        upgradeMenu = transform.GetChild(0).gameObject;
        turret = this.gameObject;
        upgradeMenu.gameObject.SetActive(false);
        parentObject = transform.parent.gameObject;
        currentTargetState = targetState.first;
    }

    
    void Update()
    {
        try
        {
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
                    target = getFirstEnemy(enemysInRange).transform;
                }
                catch (MissingReferenceException)
                {
                
                }

            }
            if (Vector3.Distance(transform.position, target.position) <= attackRadius)
            {
                
                if (Time.time >= lastFireTime + fireRate)
                {
                    Vector3 interceptPoint = CalculateInterceptPoint();
                    if (interceptPoint != Vector3.zero)
                    {
                        FireProjectile(interceptPoint);
                        lastFireTime = Time.time; // Update the last fire time
                    }
                    
                }

                parentObject.transform.LookAt(target.transform, Vector3.up);
            }

            

        }



        if (Input.GetMouseButtonDown(0))
        {
            if (turret == GetClickedObject(out RaycastHit hit))
            {
                print("clicked/touched!");
                upgradeMenu.gameObject.SetActive(true);

                //if time stops add here
            }
        }

    }



    GameObject GetClickedObject(out RaycastHit hit)
    {
        GameObject target = null;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
        {
            if (!isPointerOverUIObject())
            {
                target = hit.collider.gameObject;
            } 
            //might need to add if (!isPointerOverUIObject()) {}
        }
        return target;
    }

    private bool isPointerOverUIObject()     //avoids the click interacting with ui
    {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);
        return results.Count > 0;
    }



    Vector3 CalculateInterceptPoint()
    {
        Vector3 toTarget = target.position - transform.position;
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
        return target.position + targetVelocity * interceptTime;
    }

    void FireProjectile(Vector3 interceptPoint)
    {
        Vector3 direction = (interceptPoint - transform.position).normalized;
        Transform projectile = Instantiate(Bullet.transform, transform.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
    }
    public void Upgrade1()
    {
        print("upgrade 1");
        //input whatever upgrade we want
    }

    public void Upgrade2()
    {
        print("upgrade 2");
        //input whatever upgrade we want
    }

    public void Upgrade3()
    {
        print("upgrade 3");
        //input whatever upgrade we want
    }

    public void close()
    {
        upgradeMenu.gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        //sets current target only if its not already targeting something
        if (other.CompareTag("dude"))
        {
            enemysInRange.Add(other.gameObject);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(enemysInRange.Contains(other.gameObject))
        {
            enemysInRange.Remove(other.gameObject);
        }
    }

    GameObject getFirstEnemy(List<GameObject> wap)
    {
        GameObject firstEnemy = wap[0];
        foreach (GameObject enemy in wap)
        {
            try
            {
                if (int.Parse(enemy.name) < int.Parse(firstEnemy.name))
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
