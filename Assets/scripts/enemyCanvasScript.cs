using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyCanvasScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        this.transform.LookAt(Camera.main.transform.position);
    }

}
