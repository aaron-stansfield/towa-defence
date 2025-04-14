using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bullet : MonoBehaviour
{
    public int health; // how many enemies a bullet can hit before dying
    public bool chanceExplosive; //if true bullet has a chance to explode on contact
    public bool explosive;
    public bool isExplosion; // true if this bullet is the explosion from another bullet
    public GameObject bigBullet;
    public float bulletLifeTime; // life time in seconds
    public bool parabolic; // if true projectile will shoot in arc
    private Rigidbody rb;
    public bool slows; // if true bullet has chance to slow enemies on hit
    public bool extraSlowChance; // if true higher chance to slow enemies on hit
    public int explosionDamage;
    public bool sauce;
    public GameObject sauceObj;
    public float sauceLifeSpan;
    public Vector3 angle;
    public Mesh hotdogMesh;
    public int bulletDamage;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        StartCoroutine(BulletLifeTime());
        
    }

    private void Update()
    {
        if (!parabolic)
        {
            this.rb.velocity = new Vector3(this.rb.velocity.x, 0, this.rb.velocity.z);
        }
        else
        {
            //this.transform.rotation = Quaternion.Euler(this.rb.velocity.y * 5, this.transform.rotation.y - 40, 0);
            this.transform.rotation = Quaternion.LookRotation(rb.velocity);
            this.GetComponent<MeshFilter>().mesh = hotdogMesh;
        }
        if (health <= 0)
        {
            Destroy(gameObject);
        }

    }
    public IEnumerator BulletLifeTime()
    {
        yield return new WaitForSeconds(bulletLifeTime);
        Destroy(gameObject);
    }


    public void OnTriggerEnter(Collider other)
    {
        System.Random random = new System.Random();
        int chance = random.Next(0, 5);

        if (other.CompareTag("dude") && health > 0 && !explosive)
        {
            other.gameObject.GetComponent<enmy_scrip>().damaged(bulletDamage); // Ensure bulletDamage is passed here.
            health--;
            //Debug.Log($"Bullet damage set to: {bulletDamage}");
            if (chanceExplosive && !isExplosion)
            {
               if (chance == 0)
               {
                    Transform explosion = Instantiate(bigBullet.transform);
                    explosion.gameObject.transform.localScale = new Vector3(10, 10, 10);
                    explosion.GetComponent<Rigidbody>().useGravity = false;
                    explosion.GetComponent<SphereCollider>().radius = 0.7f;
                    explosion.GetComponent<Bullet>().isExplosion = true;
                    explosion.GetComponent<Bullet>().health = explosionDamage;
                    explosion.GetComponent<Bullet>().bulletLifeTime = 0.3f;
                
               }
               
            }
            health--;
        }

        else if (other.CompareTag("ground") && !isExplosion && explosive)
        {
            Transform explosion = Instantiate(bigBullet.transform);
            explosion.gameObject.transform.localScale = new Vector3(0, 0, 0);
            explosion.GetComponent<Rigidbody>().useGravity = false;
            explosion.GetComponent<SphereCollider>().radius = 0.7f;
            explosion.transform.position = new Vector3(explosion.transform.position.x, 1.7f, explosion.transform.position.z);
            explosion.GetComponent<Bullet>().isExplosion = true;
            explosion.GetComponent<Bullet>().health = explosionDamage;
            explosion.GetComponent<Bullet>().bulletLifeTime = 0.3f;
            Transform sauceInstance = Instantiate(sauceObj.transform);
            sauceInstance.transform.position = new Vector3(this.transform.position.x,1.7f,this.transform.position.z);
            sauceInstance.GetComponent<sauceScript>().lifeTime = sauceLifeSpan;
        }
        

        if ((chance == 1 && slows) || (slows && extraSlowChance && (chance == 1 || chance == 2)))
        {
            other.GetComponent<enmy_scrip>().slowed();
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (isExplosion && other.CompareTag("dude") && health >= 0)
        {
            other.gameObject.GetComponent<enmy_scrip>().damaged(1);
            health--;
        }
    }

}
