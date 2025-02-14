using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletLifeTime;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("BulletLifeTime");
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
