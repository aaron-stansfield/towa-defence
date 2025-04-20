using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sauceScript : MonoBehaviour
{
    public float lifeTime;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(lifeSpan());         //way better
    }

    private void Awake()
    {
        
    }
     //Update is called once per frame
    void Update()
    {
        //StartCoroutine(lifeSpan());
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("dude"))
        {
            other.GetComponent<enmy_scrip>().slowed();
        }
    }

    private IEnumerator lifeSpan()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(this.gameObject);
    }
}
