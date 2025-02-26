using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class game_managie : MonoBehaviour
{
    private TextMeshProUGUI healthText;
    public GameObject enmy;
    public GameObject spawnpoint;
    private int enmyCount = 0;
    public float spawnDelay;
    public List<GameObject> towerList;
    public List<GameObject> enemyList;
    public int baseTowerCost = 10;
    public TextMeshProUGUI baseTowerCostText;
    public int arcerTowerCost = 50;
    public int wackerTowerCost = 75;
    public bool anyUpgradeMenuOpen;
    public int money = 10;
    public int deathCount = 0;
    public GameObject towerPurchaseButtons;
    public TextMeshProUGUI moneyText;
    public GameObject gameUI;
    

    public float enmySpeed;


    public int enemyHealth;

    // Added for pause functionality
    public GameObject pauseMenuUI;
    [SerializeField] private bool isPaused = false; // for when the player uses the UI pause button just to stop time
    public bool isProperPaused = false; // for when the player presses escape and the whole game stops && menu is shown

    void Start()
    {
        healthText = GameObject.Find("healthAmount").GetComponent<TextMeshProUGUI>();
        StartCoroutine(enmy_spawner());
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
        if (deathCount < 20)
        {
            spawnDelay = 1.7f;         //1.7
            enemyHealth = 3;

        }
        else if (deathCount < 50)
        {
            spawnDelay = 0.6f;
            enemyHealth = 6;
        }
        else if (deathCount < 100)
        {
            spawnDelay = 0.5f;
            enemyHealth = 8;
        }
        else if (deathCount < 300)
        {
            spawnDelay = 0.2f;
            enemyHealth = 10;
        }
        else if (deathCount < 500)
        {
            spawnDelay = 0.1f;
            enemyHealth = 12    ;
        }

        moneyText.text = money.ToString();
        baseTowerCostText.text = baseTowerCost.ToString();

        if (anyUpgradeMenuOpen)
        {
            towerPurchaseButtons.SetActive(false);
        }
        else
        {
            towerPurchaseButtons.SetActive(true);
        }
    }

    IEnumerator enmy_spawner()
    {
        yield return new WaitForSeconds(spawnDelay);
        GameObject dude = Instantiate(enmy, spawnpoint.transform);
        dude.name = enmyCount.ToString();
        dude.GetComponent<NavMeshAgent>().speed = enmySpeed;
        enmyCount++;
        StartCoroutine(enmy_spawner());
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
