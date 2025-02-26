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
    public float stunTime;
    public bool inStunZone;
    private float givenSpeed;
    public bool isStunned;

    // Start is called before the first frame update
    public void Start()
    {
        //hammer tower
       

        GameObject gameManagerObj = GameObject.Find("game managie");
        gameManager = gameManagerObj.GetComponent<game_managie>();
        goal = GameObject.FindGameObjectWithTag("goal");
        agent = this.GetComponent<NavMeshAgent>();
        agent.SetDestination(goal.transform.position);
        health = gameManager.enemyHealth;
        
        hammerTowerScript = GameObject.FindGameObjectWithTag("hammer").GetComponent<hammerScript>();

        givenSpeed = this.GetComponent<NavMeshAgent>().speed;
    }

    // Update is called once per frame
    void Update()
    {
        //if (inStunZone == true && hammerTowerScript.toStun == true)
        //{
            //Stun();

            //print("should Stun");
            //this.GetComponent<NavMeshAgent>().speed = 0;
            //hammerTowerScript.toStun = false;
        
            //Invoke(nameof(Unstun),stunTime);
        ///}

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
    }

    private void OnTriggerStay(Collider other)
    {
        

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
        if (!isStunned)
        {
            isStunned = true;
            print("should Stun");
            this.GetComponent<NavMeshAgent>().speed = 0;

            //hammerTowerScript.toStun = false;       

            Invoke(nameof(Unstun), stunTime);
        }
        

    }

    public void Unstun()
    {
        isStunned = false;
        this.GetComponent<NavMeshAgent>().speed = givenSpeed;
        print("unstun");

    }
}

    
