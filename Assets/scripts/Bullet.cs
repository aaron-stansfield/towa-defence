using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;
    public float bulletLifeTime;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        StartCoroutine("BulletLifeTime");
    }

    private void Update()
    {
        this.rb.velocity = new Vector3(this.rb.velocity.x, 0 , this.rb.velocity.z);
    }
    public IEnumerator BulletLifeTime()
    {
        yield return new WaitForSeconds(bulletLifeTime);
        Destroy(gameObject);
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "dude")
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
