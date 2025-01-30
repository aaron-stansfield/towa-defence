using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class game_managie : MonoBehaviour
{


    public GameObject enmy;
    public GameObject spawnpoint;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(enmy_spawner());
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    IEnumerator enmy_spawner()
    {
        yield return new WaitForSeconds(1);
        Instantiate(enmy,spawnpoint.transform);
        StartCoroutine(enmy_spawner());
    }

}
