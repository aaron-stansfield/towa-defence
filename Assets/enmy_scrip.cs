using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class enmy_scrip : MonoBehaviour
{
    public GameObject goal;
    private NavMeshAgent agent;
    private game_managie gameManager;
    // Start is called before the first frame update
    public void Start()
    {
        GameObject gameManagerObj = GameObject.Find("game managie");
        gameManager = gameManagerObj.GetComponent<game_managie>();
        goal = GameObject.FindGameObjectWithTag("goal");
        agent = this.GetComponent<NavMeshAgent>();
        agent.SetDestination(goal.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        moveEnmy();
    }

    public void moveEnmy()
    {
        agent.SetDestination(goal.transform.position);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("goal"))
        {
            gameManager.Damage();
            Destroy(this.gameObject);
        }
    }
}
