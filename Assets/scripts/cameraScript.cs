using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static game_managie;

public class cameraScript : MonoBehaviour
{

    public LayerMask groundMask;
    public GameObject towerToPlace;
    public GameObject managerTemp;
    private game_managie manager;
    private GameObject towerGhost;
    public mouseState currentMouseState;
    
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
                towerGhost.transform.position = hit.point;
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject newTower = Instantiate(towerToPlace);
                    newTower.transform.position = hit.point;
                    Destroy(towerGhost);
                    changeMouseState();
                }
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


    public void baseTowerSelect()
    {
        towerToPlace = manager.towerList[0];
        towerGhost = Instantiate(manager.towerList[1]);
        changeMouseState();
    }
}
