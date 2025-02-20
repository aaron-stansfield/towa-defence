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
    public int baseTowerCost = 5;
    public TextMeshProUGUI baseTowerCostText;
    public bool anyUpgradeMenuOpen;
    public int money = 5;
    public int deathCount = 0;
    public GameObject towerPurchaseButtons;
    public TextMeshProUGUI moneyText;

    public float enmySpeed;


    public int enemyHealth;

    // Added for pause functionality
    public GameObject pauseMenuUI; // Assign a UI Panel in Unity for pause menu
    private bool isPaused = false;

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
            TogglePause();
        }
    }

    void FixedUpdate()
    {
        if (deathCount < 20)
        {
            spawnDelay = 1.7f;
            enemyHealth = 5;

        }
        else if (deathCount < 50)
        {
            spawnDelay = 0.6f;
            enemyHealth = 7;
        }
        else if (deathCount < 100)
        {
            spawnDelay = 0.5f;
            enemyHealth = 12;
        }
        else if (deathCount < 300)
        {
            spawnDelay = 0.2f;
            enemyHealth = 12;
        }
        else if (deathCount < 500)
        {
            spawnDelay = 0.05f;
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

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0; // Pause game
            if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
        }
        else
        {
            Time.timeScale = 1; // Resume game
            if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1; // Unpause game
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
    }
}
