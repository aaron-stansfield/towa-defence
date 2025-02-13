using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TurretUpgrade : MonoBehaviour
{
    // Start is called before the first frame update
     private GameObject turret;
     public GameObject upgradeMenu;

    // Start is called before the first frame update
    void Start()
    {
     turret = this.gameObject;
     upgradeMenu.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (turret == GetClickedObject(out RaycastHit hit))
            {
                print("clicked/touched!");
                upgradeMenu.gameObject.SetActive(true);

                //if time stops add here
            }
        }
    }

    GameObject GetClickedObject(out RaycastHit hit)
    {
        GameObject target = null;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
        {
            if (!isPointerOverUIObject()) { target = hit.collider.gameObject; } //might need to add if (!isPointerOverUIObject()) {}
        }
        return target;
    }
    private bool isPointerOverUIObject()                                    //avoids the click interacting with ui
    {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);
        return results.Count > 0;
    }

    public void  Upgrade1()
    {
        print("upgrade 1");      
        //input whatever upgrade we want
    }

    public void  Upgrade2()
    {
        print("upgrade 2");     
        //input whatever upgrade we want
    }

    public void  Upgrade3()
    {
        print("upgrade 3");     
        //input whatever upgrade we want
    }

    public void close()
    {
        upgradeMenu.gameObject.SetActive(false);
    }
}

