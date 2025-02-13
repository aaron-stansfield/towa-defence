using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("BulletLifeTime");
    }

    public IEnumerator BulletLifeTime()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }


    public void OnTriggerEnter(Collider co)
    {
        if (co.gameObject.tag == "enemy")
        {
            Destroy(gameObject);
        }
    }
}
