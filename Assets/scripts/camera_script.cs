using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static game_managie;

public class cameraScript : MonoBehaviour
{

    public LayerMask groundMask;
    public GameObject towerToPlace;
    public GameObject managerTemp;
    private game_managie manager;
    public GameObject towerGhost;
    public mouseState currentMouseState;
    private float checkRadius = 2.3f;
    public bool movingArcerTarget;
    
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
        if(currentMouseState == mouseState.placing && !manager.isProperPaused)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask) )
            {
                //creates ghost of tower to be placed if in placing mode
                towerGhost.transform.position = hit.point;
                if (!placementCheck(hit.point) && !movingArcerTarget)
                {
                    if (towerGhost)
                    towerGhost.transform.GetChild(1).GetComponent<MeshRenderer>().material.SetColor("_BaseColour",Color.red);
                }
                else
                {
                    if (!movingArcerTarget)
                    {
                        Color colour;
                        if (UnityEngine.ColorUtility.TryParseHtmlString("#5C0099", out colour))
                        {
                            towerGhost.transform.GetChild(1).GetComponent<MeshRenderer>().material.SetColor("_BaseColour", colour);
                        }

                    }
                    towerGhost.gameObject.SetActive(true);
                    if (Input.GetMouseButtonUp(0) && !isPointerOverUIObject() && !movingArcerTarget)
                    {
                        GameObject newTower = Instantiate(towerToPlace);
                        manager.money -= manager.baseTowerCost;
                        //manager.baseTowerCost += Mathf.RoundToInt(manager.baseTowerCost / 0.8f);
                        newTower.transform.position = hit.point;
                        Destroy(towerGhost);

                        changeMouseState();
                    }
                }
                if (Input.GetMouseButtonUp(0) && movingArcerTarget && !isPointerOverUIObject())
                {
                    movingArcerTarget = false;
                    towerGhost = null;

                    changeMouseState();
                }

            }
            if (Input.GetMouseButton(1) && !movingArcerTarget)
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
        if (currentMouseState != mouseState.placing && !manager.isProperPaused)
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

    public void arcerTowerSelect()
    {
        if (currentMouseState != mouseState.placing && !manager.isProperPaused)
        {
            towerToPlace = manager.towerList[2];
            if (manager.money >= manager.arcerTowerCost)
            {
                towerGhost = Instantiate(manager.towerList[3]);
                towerGhost.gameObject.SetActive(false);
                changeMouseState();
            }

        }
    }

    public void wackerTowerSelect()
    {
        if (currentMouseState != mouseState.placing && !manager.isProperPaused)
        {
            towerToPlace = manager.towerList[4];
            if (manager.money >= manager.wackerTowerCost)
            {
                towerGhost = Instantiate(manager.towerList[5]);
                towerGhost.gameObject.SetActive(false);
                changeMouseState();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(towerGhost.gameObject.transform.position , checkRadius);
    }

    private bool isPointerOverUIObject()  //avoids the click interacting with ui
    {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);
        return results.Count > 0;
    }
}
