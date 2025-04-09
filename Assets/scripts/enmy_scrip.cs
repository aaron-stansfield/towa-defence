using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class enmy_scrip : MonoBehaviour
{
    //for hammer tower
    hammerScript hammerTowerScript;

    private bool isDead;
    private GameObject goal;
    private GameObject startPoint;
    private NavMeshAgent agent;
    private game_managie gameManager;
    public int health;
    public int moneyOnDeath;
    public int moneyOnHit;
    public float slowTime;
    public GameObject slowFX;
    public GameObject AnimCanvas;
    public float stunTime;
    public bool inStunZone;
    private float givenSpeed;
    public bool isStunned;
    private bool isSlowed;

    // Start is called before the first frame update
    public void Start()
    {
        //hammer tower

        //AnimCanvas.GetComponent<Animation>().Play("2DDudeRun");
        GameObject gameManagerObj = GameObject.Find("game managie");
        gameManager = gameManagerObj.GetComponent<game_managie>();
        startPoint = GameObject.FindGameObjectWithTag("startPoint");
        goal = GameObject.FindGameObjectWithTag("goal");
        agent = this.GetComponent<NavMeshAgent>();
        agent.SetDestination(goal.transform.position);
        health = gameManager.enemyHealth;
        
        //hammerTowerScript = GameObject.FindGameObjectWithTag("hammer").GetComponent<hammerScript>();

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
        if (!isDead && !isStunned)
        {
            moveEnmy();
        }

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
            
            gameManager.deathCount++;
            //gameManager.money += moneyOnDeath;

            
            if (gameManager.enemyList.Contains(this.gameObject))
            {
                gameManager.enemyList.Remove(this.gameObject);
            }

            StartCoroutine(delayedDeath());

            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        

        if (other.CompareTag("stunZone"))
        {
           
           inStunZone = true;
        }
    }


    IEnumerator delayedDeath()
    {
        isDead = true;
        
        this.gameObject.tag = "null";
        this.gameObject.layer = 0;
        Destroy(this.GetComponent<Rigidbody>());
        Destroy(this.GetComponent<CapsuleCollider>());
        Destroy(this.GetComponent<NavMeshAgent>());
        
        //this.GetComponent<NavMeshAgent>().IsDestroyed();
        AnimCanvas.GetComponent<Animator>().SetTrigger("Dead");
        yield return new WaitForSeconds(0.6f);
        Destroy(this.gameObject);
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
                StartCoroutine(delayedDeath());
                
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
        if (!isSlowed)
        {
            isSlowed = true;
            this.GetComponent<NavMeshAgent>().speed = this.GetComponent<NavMeshAgent>().speed / 2;
            Color color;
            UnityEngine.ColorUtility.TryParseHtmlString("#FF82C8", out color);
            AnimCanvas.GetComponent<SpriteRenderer>().color = color;
            //slowFX.gameObject.SetActive(true);
            yield return new WaitForSeconds(slowTime);
            this.GetComponent<NavMeshAgent>().speed = this.GetComponent<NavMeshAgent>().speed * 2;
            AnimCanvas.GetComponent<SpriteRenderer>().color = Color.white;
            isSlowed = false;
            //slowFX.gameObject.SetActive(false);
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

    public IEnumerator KnockBack()
    {
        isStunned = true;
        agent.SetDestination(startPoint.transform.position);
        agent.speed = givenSpeed * 2;
        yield return new WaitForSeconds(1);
        agent.SetDestination(goal.transform.position);
        agent.speed = givenSpeed / 2;
        isStunned = false;
        
        
    }

    public void Unstun()
    {
        isStunned = false;
        this.GetComponent<NavMeshAgent>().speed = givenSpeed;
        print("unstun");

    }
}

    
