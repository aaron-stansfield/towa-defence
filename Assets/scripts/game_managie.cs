using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class game_managie : MonoBehaviour
{
    private TextMeshProUGUI healthText;
    public GameObject enmy;
    public GameObject properEnmy;
    public GameObject spawnpoint;
    private int enmyCount = 0;
    public float spawnDelay;
    public List<GameObject> towerList;
    public List<GameObject> enemyList;
    public int baseTowerCost = 100;
    public TextMeshProUGUI baseTowerCostText;
    public int arcerTowerCost = 200;
    public int wackerTowerCost = 250;
    public bool anyUpgradeMenuOpen;
    public int money = 100;
    public int deathCount = 0;
    public GameObject towerPurchaseButtons;
    public TextMeshProUGUI moneyText;
    public GameObject gameUI;
    private bool isTowerUIHidden;
    public Sprite UIPause;
    public Sprite UIPlay;
    public GameObject pauseButton;

    public TMP_Text TowerCostBase;
    public TMP_Text TowerCostArcer;
    public TMP_Text TowerCostWacker;
    public TMP_Text waveCount;
    private int currentWave = 1;
    private int totalDudes = 0;
    private int roundStartingHealth;

    public List<int> roundsAmount = new List<int>
    {
         10,
         20,
         50,
         70,
         100,
         300
    };

    public List<int> roundsHealth = new List<int>
    {
         2,
         3,
         5,
         10,
         15,
         20
    };

    public List<float> roundsSpawnDelay = new List<float>
    {
         1.7f,
         1.7f,
         1.5f,
         1.3f,
         1.1f,
         1.1f
    };


    public float enmySpeed;


    public int enemyHealth;

    // Added for pause functionality
    public GameObject pauseMenuUI;
    [SerializeField] private bool isPaused = false; // for when the player uses the UI pause button just to stop time
    public bool isProperPaused = false; // for when the player presses escape and the whole game stops && menu is shown

    void Start()
    {
        TogglePause(false);
        healthText = GameObject.Find("healthAmount").GetComponent<TextMeshProUGUI>();
        StartCoroutine(WaveHandler());
        
        TowerCostBase.text = baseTowerCost.ToString();
        TowerCostArcer.text = arcerTowerCost.ToString();
        TowerCostWacker.text = wackerTowerCost.ToString();

        if(GameObject.Find("nothin 2 worry bout").GetComponent<nothin>().cooldudes)
        {
            enmy = properEnmy;
        }
    }

    void Update()
    {
        // Toggle pause when ESC key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause(true);
        }
    }

    void FixedUpdate()
    {
        moneyText.text = money.ToString();
        baseTowerCostText.text = baseTowerCost.ToString();

        if (anyUpgradeMenuOpen)
        {
            
            //towerPurchaseButtons.SetActive(false);
        }
        else
        {
            //towerPurchaseButtons.SetActive(true);
        }
    }

    public void speedToggle()
    {
        if (Time.timeScale == 1 || Time.timeScale == 0)
        {
            if (isPaused)
            {
                TogglePause(false);
            }
            Time.timeScale = 2;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    IEnumerator WaveHandler()
    {
        while (true)
        {
            int spawnedThisWave = 0;
            int killsAtStartOfRound = deathCount;
            roundStartingHealth = int.Parse(healthText.text);
            for (int i = 0; i < roundsAmount[currentWave - 1]; i++)
            {
                yield return new WaitForSeconds(spawnDelay);
                spawnedThisWave++;
                enmy_spawn();
                
            }
            Debug.Log(spawnedThisWave);
            //for (int i = 0; i < currentWave; i++)
            //{
            //    totalDudes += roundsAmount[i];
            //}
            while ((deathCount - killsAtStartOfRound) + Mathf.Abs(int.Parse(healthText.text) - roundStartingHealth) < roundsAmount[currentWave - 1])
            {
                //Debug.Log(totalDudes);
                //Debug.Log(roundsAmount[])
                
                Debug.Log(roundStartingHealth);
                yield return new WaitForSeconds(0.3f);
            }
            //totalDudes = 0;
            TogglePause(false);
            currentWave++;
            spawnDelay = roundsSpawnDelay[currentWave];
            enemyHealth = roundsHealth[currentWave];
            
            waveCount.text = currentWave.ToString();
        }
        //for (int i = 0; i < round2Length + 1; i++)
        //{
        //    StartCoroutine(enmy_spawn());
        //    yield return new WaitForSeconds(spawnDelay);
        //}
        //while (deathCount + Mathf.Abs(int.Parse(healthText.text) - 100) < (round2Length + round1Length + 2))
        //{
        //    yield return new WaitForSeconds(0.3f);
        //}
        //TogglePause(false);
        //spawnDelay = 1.7f;
        //enemyHealth = 3;
        //currentWave++;
        //waveCount.text = currentWave.ToString();

        //for (int i = 0; i < round3Length + 1; i++)
        //{
        //    StartCoroutine(enmy_spawn());
        //    yield return new WaitForSeconds(spawnDelay);
        //}

        //while (deathCount + Mathf.Abs(int.Parse(healthText.text) - 100) < (round3Length + round2Length + round1Length + 2))
        //{
        //    yield return new WaitForSeconds(0.3f);
        //}
    }

    private void enmy_spawn()
    {
        GameObject dude = Instantiate(enmy, spawnpoint.transform);
        dude.name = enmyCount.ToString();
        dude.GetComponent<NavMeshAgent>().speed = enmySpeed;
        enmyCount++;

    }

    public void PlayTowerUIAnimation()
    {
        if (isTowerUIHidden)
        {

            towerPurchaseButtons.GetComponent<Animation>().Play("UIunshlorp");
        }
        else
        {
            towerPurchaseButtons.GetComponent<Animation>().Play("UIshlorp");
        }

        isTowerUIHidden = !isTowerUIHidden;
    }

    public void Damage()
    {
        int amount = int.Parse(healthText.text);
        amount -= 1;
        healthText.text = amount.ToString();

        if (amount <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over!"); // Debug log for testing

        // Pause the game instead of quitting
        Time.timeScale = 0;

        // Show game over UI if available
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
        // This is temp for now, I want to be able to load a scene eg game over scene or main menu scene 
    }

    public void TogglePause(bool properPause)
    {
        if (properPause)
        {
            if (!isProperPaused) // pause and show menu
            {
                pauseMenuUI.SetActive(true);
                isProperPaused = true;
                gameUI.SetActive(false);
                Time.timeScale = 0;
            }
            else //unpause and hide menu
            {
                pauseMenuUI.SetActive(false);
                isProperPaused = false;
                gameUI.SetActive(true);
                Time.timeScale = !isPaused ? 1 : 0;
            }
        }
        else
        {
            if (!isPaused)
            {

                pauseButton.GetComponent<Image>().sprite = UIPlay;

            }
            else
            {

                pauseButton.GetComponent<Image>().sprite = UIPause;
            }
            Time.timeScale = isPaused ? 1 : 0; // toggle time pause
            isPaused = !isPaused;
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1; // Unpause game
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);

    }
}
