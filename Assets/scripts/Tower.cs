using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Vector3 tarDir;
    public GameObject enemy;
    public GameObject Tower1;
    public bool ActiveTower = true;
    public float speed;
    public float ShootTime;
    public float MaxShootTime;
    public GameObject Bullet;
    public GameObject BSpawn;
    public float Force;
    public float TrackingAngle;
    public float TrackingDist;

    
    void Start()
    {
        enemy = GameObject.FindGameObjectWithTag("enemy");
      //  StartCoroutine("ReachCheck");

    }

    
    void Update()
    {
        if (ActiveTower == true)
        {
            Debug.Log("woo");
            tarDir = enemy.transform.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, tarDir, speed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);

            if (ShootTime > MaxShootTime)

            {
                var bullet = Instantiate(Bullet, BSpawn.transform.position, BSpawn.transform.rotation);

                bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * Force);
                ShootTime = 0;

            }

            ShootTime += Time.deltaTime;

        }
      }

    private bool EnemyInSight()
    {
        float ab = Vector3.Dot(transform.forward, (enemy.transform.position - transform.position).normalized);

        if (ab > TrackingAngle)
        
            return true;
          return false;
        
    }

    private bool EnemyReachable()
    {
        float reach = Vector3.Distance(enemy.transform.position, transform.position);

        if (reach < TrackingDist)
            return true;
        return false;
    }


  //public IEnumerator ReachCheck()
    //{
      // for (; ;)
      // {
        //    ActiveTower = EnemyReachable() && EnemyInSight();

          //  yield return new WaitForSeconds(0.1f);
       // }
    

        
   // }
}
