using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canvasScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
