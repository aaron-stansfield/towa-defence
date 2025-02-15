using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bullet : MonoBehaviour
{
    public int health; // how many enemies a bullet can hit before dying
    public bool explosive; //if true bullet has a chance to explode on contact
    public bool isExplosion; // true if this bullet is the explosion from another bullet
    public GameObject bigBullet;
    public float bulletLifeTime;
    public bool parabolic; // if true projectile will shoot in arc
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        StartCoroutine("BulletLifeTime");
    }

    private void Update()
    {
        if (!parabolic)
        {
            this.rb.velocity = new Vector3(this.rb.velocity.x, 0, this.rb.velocity.z);
        }
        
    }
    public IEnumerator BulletLifeTime()
    {
        yield return new WaitForSeconds(bulletLifeTime);
        Destroy(gameObject);
    }


    public void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "dude" && health > 0)
        {
            other.gameObject.GetComponent<enmy_scrip>().damaged();
            if (explosive && !isExplosion)
            {
               System.Random random = new System.Random();
               int chance = random.Next(0,5);
               if (chance == 0)
               {
                    Transform explosion = Instantiate(bigBullet.transform);
                    explosion.gameObject.transform.localScale = new Vector3(10, 10, 10);
                    explosion.GetComponent<Bullet>().isExplosion = true;
                    explosion.GetComponent<Bullet>().health = 20;
                    explosion.GetComponent<Bullet>().bulletLifeTime = 0.3f;
                
               }
            }
            health--;
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }
        
    }

}
