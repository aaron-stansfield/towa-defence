using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static game_managie;

public class cameraScript : MonoBehaviour
{

    public LayerMask groundMask;
    public GameObject towerToPlace;
    public GameObject managerTemp;
    private game_managie manager;
    private GameObject towerGhost;
    public mouseState currentMouseState;
    private float checkRadius = 1.3f;
    
    public enum mouseState
    {
        normal,
        placing
    }
    // Start is called before the first frame update
    void Start()
    {
        manager = managerTemp.GetComponent<game_managie>();
        currentMouseState = mouseState.normal;
    }

    // Update is called once per frame
    void Update()
    {
        inputStuff();
    }


    private void inputStuff()
    {
        if(currentMouseState == mouseState.placing)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
            {
                //creates ghost of tower to be placed if in placing mode
                //Vector3 tempPos = hit.point;
                towerGhost.transform.position = hit.point;
                if (!placementCheck(hit.point))
                {
                    towerGhost.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.red;
                }
                else
                {
                    towerGhost.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.black;
                    towerGhost.gameObject.SetActive(true);
                    if (Input.GetMouseButtonUp(0) && !isPointerOverUIObject())
                    {
                        GameObject newTower = Instantiate(towerToPlace);
                        manager.money -= manager.baseTowerCost;
                        //manager.baseTowerCost += Mathf.RoundToInt(manager.baseTowerCost / 0.8f);
                        newTower.transform.position = hit.point;
                        Destroy(towerGhost);
                        
                        changeMouseState();

                    }
                }

                
                
            }
            
            if (Input.GetMouseButton(1))
            {
                Destroy(towerGhost);
                changeMouseState();
            }
        }
        
    }

    public void changeMouseState()
    {
        if (currentMouseState == mouseState.normal)
        {
            currentMouseState = mouseState.placing;
        }
        else
        {
            currentMouseState = mouseState.normal;
        }

    }

    public bool placementCheck(Vector3 pos)
    {
        Collider[] currentObjects = Physics.OverlapSphere(pos, checkRadius);
        
        foreach(Collider col in currentObjects)
        {
            if (col.gameObject.CompareTag("towerCollider") || col.gameObject.CompareTag("navMesh"))
            {
                return false;
            }
        }
        
        return true;
    }

    public void baseTowerSelect()
    {
        if (currentMouseState != mouseState.placing)
        {
            towerToPlace = manager.towerList[0];
            if (manager.money >= manager.baseTowerCost)
            {
                towerGhost = Instantiate(manager.towerList[1]);
                towerGhost.gameObject.SetActive(false);
                changeMouseState();
            }
            
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawSphere(towerGhost.transform.position, checkRadius);
    //}

    private bool isPointerOverUIObject()  //avoids the click interacting with ui
    {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);
        return results.Count > 0;
    }
}
