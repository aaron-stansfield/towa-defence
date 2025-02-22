using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class enmy_scrip : MonoBehaviour
{
    //for hammer tower
    hammerScript hammerTowerScript;


    public GameObject goal;
    private NavMeshAgent agent;
    private game_managie gameManager;
    public int health;
    public int moneyOnDeath;
    public int moneyOnHit;
    public float slowTime;
    public GameObject slowFX;
    public bool toStun;
    public float stunTime;
    public bool inStunZone;

    // Start is called before the first frame update
    public void Start()
    {
        //hammer tower
        hammerTowerScript = GameObject.FindGameObjectWithTag("hammer").GetComponent<hammerScript>();

        GameObject gameManagerObj = GameObject.Find("game managie");
        gameManager = gameManagerObj.GetComponent<game_managie>();
        goal = GameObject.FindGameObjectWithTag("goal");
        agent = this.GetComponent<NavMeshAgent>();
        agent.SetDestination(goal.transform.position);
        health = gameManager.enemyHealth;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
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
            if (gameManager.enemyList.Contains(this.gameObject))
            {
                gameManager.enemyList.Remove(this.gameObject);
            }
            Destroy(this.gameObject);
        }

        if (other.CompareTag("stunZone") && hammerTowerScript.toStun == true)
        {
            print("should Stun");
            this.GetComponent<NavMeshAgent>().speed = 0;
        
            Invoke(nameof(Unstun),stunTime);
        }

        if (other.CompareTag("stunZone"))
        {
            inStunZone = true;
        }
    }

    public void damaged(int damageDone)
    {
        health -= damageDone;
        {
            if (health <= 0)
            {
                gameManager.deathCount++;
                gameManager.money += moneyOnDeath;
                if (gameManager.enemyList.Contains(this.gameObject))
                {
                    gameManager.enemyList.Remove(this.gameObject);
                }
                Destroy(this.gameObject);
            }
            gameManager.money += moneyOnHit;
        }
    }

    public void slowed()
    {
        StartCoroutine(slowedIEnum());
    }
    private IEnumerator slowedIEnum()
    {
        if (!slowFX.activeInHierarchy)
        {
            this.GetComponent<NavMeshAgent>().speed = this.GetComponent<NavMeshAgent>().speed / 2;
            slowFX.gameObject.SetActive(true);
            yield return new WaitForSeconds(slowTime);
            this.GetComponent<NavMeshAgent>().speed = this.GetComponent<NavMeshAgent>().speed * 2;
            slowFX.gameObject.SetActive(false);
        }
    }

    public void Stun()
    {
        print("should Stun");
        this.GetComponent<NavMeshAgent>().speed = 0;
        
        Invoke(nameof(Unstun),stunTime);

    }

    public void Unstun()
    {
        this.GetComponent<NavMeshAgent>().speed = this.GetComponent<NavMeshAgent>().speed * 2;
        hammerTowerScript.toStun = false;

    }
}

    
