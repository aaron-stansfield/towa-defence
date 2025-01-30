using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enmy_scrip : MonoBehaviour
{
    public GameObject goal;
    private NavMeshAgent agent;
    // Start is called before the first frame update
    public void Start()
    {
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
}
