using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class game_managie : MonoBehaviour
{
    private TextMeshProUGUI healthText;
    public GameObject spawnpoint;
    private int enmyCount = 0;
    public float spawnDelay;
    public List<GameObject> towerList;
    public List<GameObject> enemyList;
    public int baseTowerCost = 80;
    public TextMeshProUGUI baseTowerCostText;
    public int arcerTowerCost = 150;
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
    public GameObject roundChangeHolder;
    private bool isShlorped = false;

    public TMP_Text TowerCostBase;
    public TMP_Text TowerCostArcer;
    public TMP_Text TowerCostWacker;
    public TMP_Text waveCount;
    private int currentWave = 1;

    // 20/03/25
    public GameObject normalEnemyPrefab;
    public GameObject tankEnemyPrefab;
    public GameObject fastEnemyPrefab;

    public List<WaveConfig> waves = new List<WaveConfig>
    {
        new WaveConfig { NormalEnemies = 8, TankEnemies = 0, FastEnemies = 0, SpawnOrder = new List<string> { "Normal" } },
        new WaveConfig { NormalEnemies = 13, TankEnemies = 0, FastEnemies = 0, SpawnOrder = new List<string> { "Normal" } },
        new WaveConfig { NormalEnemies = 10, TankEnemies = 2, FastEnemies = 0, SpawnOrder = new List<string> { "Normal", "Tank" } },
        new WaveConfig { NormalEnemies = 15, TankEnemies = 4, FastEnemies = 0, SpawnOrder = new List<string> { "Tank", "Normal" } },
        new WaveConfig { NormalEnemies = 10, TankEnemies = 8, FastEnemies = 10, SpawnOrder = new List<string> { "Fast", "Tank", "Normal" } },
    };
    private int currentWaveIndex = 0;

    public float enemySpeed;
    public int enemyHealth;

    // Added for pause functionality
    public GameObject pauseMenuUI;
    [SerializeField] private bool isPaused = false; // for when the player uses the UI pause button just to stop time
    public bool isProperPaused = false; // for when the player presses escape and the whole game stops && menu is shown

    void Start()
    {
        towerPurchaseButtons.GetComponent<Animation>().Play("UIunshlorp");
        roundChangeHolder.GetComponent<Animator>().SetTrigger("Start");
        StartCoroutine(doPause());
        healthText = GameObject.Find("healthAmount").GetComponent<TextMeshProUGUI>();
        StartCoroutine(WaveHandler());

        TowerCostBase.text = baseTowerCost.ToString();
        TowerCostArcer.text = arcerTowerCost.ToString();
        TowerCostWacker.text = wackerTowerCost.ToString();
    }

    void Update()
    {
        // Toggle pause when ESC key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause(true);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            roundChangeHolder.GetComponent<Animator>().SetTrigger("Start");
        }
    }

    void FixedUpdate()
    {
        moneyText.text = money.ToString();
        baseTowerCostText.text = baseTowerCost.ToString();
    }

    public void toggleShlorp()
    {
        if (isShlorped)
        {
            towerPurchaseButtons.GetComponent<Animation>().Play("UIunshlorp");
            
        }
        else
        {
            towerPurchaseButtons.GetComponent<Animation>().Play("UIshlorp");
        }

        isShlorped = !isShlorped;
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

    public class WaveConfig
    {
        public int NormalEnemies { get; set; }
        public int TankEnemies { get; set; }
        public int FastEnemies { get; set; }
        public List<string> SpawnOrder { get; set; } // Define the order in which enemies spawn (e.g., "Normal", "Tank", "Fast")
    }

    IEnumerator WaveHandler()
    {
        while (true) // Infinite loop for both predefined waves and random mode
        {
            WaveConfig currentWave;


            if (currentWaveIndex < waves.Count)
            {
                // Use predefined wave
                currentWave = waves[currentWaveIndex];
                Debug.Log($"Starting predefined wave {currentWaveIndex}.");
            }
            else
            {
                // Generate a random wave
                currentWave = GenerateRandomWave();
                Debug.Log($"Starting random wave {currentWaveIndex}. Generated: Normal={currentWave.NormalEnemies}, Tank={currentWave.TankEnemies}, Fast={currentWave.FastEnemies}");
            }

            // Update wave count UI
            waveCount.text = $"{currentWaveIndex + 1}"; // Update the displayed wave number (+1 to make it player-friendly)

            // Reset death count for the new wave
            deathCount = 0;
            int spawnedEnemies = 0; // Track how many enemies are spawned this wave

            // Spawn enemies based on the order in the wave
            foreach (string enemyType in currentWave.SpawnOrder)
            {
                if (enemyType == "Normal")
                {
                    for (int i = 0; i < currentWave.NormalEnemies; i++)
                    {
                        yield return new WaitForSeconds(0.5f); // Adjust spawn delay
                        SpawnEnemy(normalEnemyPrefab);
                        spawnedEnemies++;
                        Debug.Log($"Spawned Normal Enemy: {spawnedEnemies} spawned so far.");
                    }
                }
                else if (enemyType == "Tank")
                {
                    for (int i = 0; i < currentWave.TankEnemies; i++)
                    {
                        yield return new WaitForSeconds(0.5f);
                        SpawnEnemy(tankEnemyPrefab);
                        spawnedEnemies++;
                        Debug.Log($"Spawned Tank Enemy: {spawnedEnemies} spawned so far.");
                    }
                }
                else if (enemyType == "Fast")
                {
                    for (int i = 0; i < currentWave.FastEnemies; i++)
                    {
                        yield return new WaitForSeconds(0.3f); // Faster spawn for fast enemies
                        SpawnEnemy(fastEnemyPrefab);
                        spawnedEnemies++;
                        Debug.Log($"Spawned Fast Enemy: {spawnedEnemies} spawned so far.");
                    }
                }
            }

            // Wait until all enemies from the current wave are handled
            while (deathCount < spawnedEnemies || enemyList.Count > 0)
            {
                enemyList.RemoveAll(enemy => enemy == null); // Clean up references to destroyed enemies
                Debug.Log($"Wave {currentWaveIndex}: Active enemies remaining: {enemyList.Count}. Death count: {deathCount}");
                yield return new WaitForSeconds(0.3f); // Periodically check
            }

            // Add a short delay to ensure any remaining enemies are truly gone
            Debug.Log($"Wave {currentWaveIndex} complete! Waiting briefly to ensure all enemies are gone.");


            //FOR CLOWN CAR!!!
            //when we add the car just adjust the time for it to be able to drive away and a new one showes up
            yield return new WaitForSeconds(3f); // Adjust this delay duration as needed


            enemyList.Clear(); // Final cleanup for any lingering references

            Debug.Log($"Wave {currentWaveIndex} officially complete. Preparing for the next wave.");
            roundChangeHolder.GetComponent<Animator>().SetTrigger("Start");

            // Trigger in-game pause
            StartCoroutine(doPause());

            // Wait for the player to unpause
            while (isPaused)
            {
                yield return null;
            }

            // Increment wave index and loop back to handle the next wave
            currentWaveIndex++;
            waveCount.text = $" {currentWave}";
        }
    }




    WaveConfig GenerateRandomWave()
    {
        WaveConfig lastWave = currentWaveIndex > 0 ? waves[Mathf.Min(currentWaveIndex - 1, waves.Count - 1)] : new WaveConfig
        {
            NormalEnemies = 8,
            TankEnemies = 0,
            FastEnemies = 0,
            SpawnOrder = new List<string> { "Normal" }
        };

        // Increase enemy counts randomly by 2-5
        int newNormalEnemies = lastWave.NormalEnemies + Random.Range(2, 10);
        int newTankEnemies = lastWave.TankEnemies + Random.Range(1, 3);
        int newFastEnemies = lastWave.FastEnemies + Random.Range(2, 6);

        // Randomize the spawn order
        List<string> newSpawnOrder = new List<string> { "Normal", "Tank", "Fast" };
        for (int i = 0; i < newSpawnOrder.Count; i++)
        {
            int swapIndex = Random.Range(0, newSpawnOrder.Count);
            string temp = newSpawnOrder[i];
            newSpawnOrder[i] = newSpawnOrder[swapIndex];
            newSpawnOrder[swapIndex] = temp;
        }

        return new WaveConfig
        {
            NormalEnemies = newNormalEnemies,
            TankEnemies = newTankEnemies,
            FastEnemies = newFastEnemies,
            SpawnOrder = newSpawnOrder
        };
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        GameObject dude = Instantiate(enemyPrefab, spawnpoint.transform);
        dude.name = enmyCount.ToString();
        enmyCount++;
    }

    IEnumerator doPause()
    {
        yield return new WaitForSeconds(0.7f);
        TogglePause(false);
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

        Time.timeScale = 0;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
    }

    public void TogglePause(bool properPause)
    {
        if (properPause)
        {
            if (!isProperPaused)
            {
                pauseMenuUI.SetActive(true);
                isProperPaused = true;
                gameUI.SetActive(false);
                Time.timeScale = 0;
            }
            else
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
            Time.timeScale = isPaused ? 1 : 0;
            isPaused = !isPaused;
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
    }
}
