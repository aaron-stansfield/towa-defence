using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    public bool roundBegin = false;

    // 20/03/25
    public GameObject normalEnemyPrefab;
    public GameObject tankEnemyPrefab;
    public GameObject fastEnemyPrefab;
    public GameObject tentCanvas;

    public GameObject startingArrows;

    public GameObject[] hurtSprites;

    public GameObject introAnim;

    //01/04/25
    public GameObject clownCar;
    public Transform startWaypoint;
    public Transform dropOffWaypoint;
    public Transform finishWaypoint;
    public int totalEnemyCount;
    private bool clownsSpawned = false; // Tracks when all the clowns in a wave have spawned
    private Coroutine activeCarSequence = null; // Tracks the active CarSequence coroutine
    private enum CarState { Idle, MovingToDropOff, Waiting, MovingToFinish, Resetting }
    private CarState currentCarState = CarState.Idle; // Start the car in an idle state



    public List<WaveConfig> waves = new List<WaveConfig>
    {
        new WaveConfig { NormalEnemies = 3, TankEnemies = 0, FastEnemies = 0, SpawnOrder = new List<string> { "Normal" } },
        new WaveConfig { NormalEnemies = 5, TankEnemies = 0, FastEnemies = 0, SpawnOrder = new List<string> { "Normal" } },
        new WaveConfig { NormalEnemies = 8, TankEnemies = 0, FastEnemies = 0, SpawnOrder = new List<string> { "Normal" } },
        new WaveConfig { NormalEnemies = 10, TankEnemies = 0, FastEnemies = 2, SpawnOrder = new List<string> { "Fast", "Normal" } },
        new WaveConfig { NormalEnemies = 14, TankEnemies = 0, FastEnemies = 0, SpawnOrder = new List<string> { "Normal" } },
        new WaveConfig { NormalEnemies = 16, TankEnemies = 0, FastEnemies = 0, SpawnOrder = new List<string> { "Normal" } },
        new WaveConfig { NormalEnemies = 15, TankEnemies = 1, FastEnemies = 2, SpawnOrder = new List<string> { "Fast", "Normal", "Tank" } },
        new WaveConfig { NormalEnemies = 10, TankEnemies = 0, FastEnemies = 4, SpawnOrder = new List<string> { "Fast", "Normal" } },
        new WaveConfig { NormalEnemies = 13, TankEnemies = 0, FastEnemies = 5, SpawnOrder = new List<string> { "Fast", "Normal" } },
        new WaveConfig { NormalEnemies = 15, TankEnemies = 3, FastEnemies = 3, SpawnOrder = new List<string> { "Fast", "Normal", "Tank" } },
    };
    private int currentWaveIndex = 0;

    public float enemySpeed;
    public int normalEnemyHealth;
    public int tankEnemyHealth;
    public int fastEnemyHealth;

    // Added for pause functionality
    public GameObject pauseMenuUI;
    [SerializeField] private bool isPaused = false; // for when the player uses the UI pause button just to stop time
    public bool isProperPaused = false; // for when the player presses escape and the whole game stops && menu is shown

    void Start()
    {
        towerPurchaseButtons.GetComponent<Animation>().Play("UIunshlorp");
        roundChangeHolder.GetComponent<Animator>().SetTrigger("Start");
        //StartCoroutine(doPause());
        healthText = GameObject.Find("healthAmount").GetComponent<TextMeshProUGUI>();
        StartCoroutine(WaveHandler());

        TowerCostBase.text = baseTowerCost.ToString();
        TowerCostArcer.text = arcerTowerCost.ToString();
        TowerCostWacker.text = wackerTowerCost.ToString();
        StartCoroutine(introAnimation());
    }

    private void OnLevelWasLoaded(int level)
    {
        Time.timeScale = 1;
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

    IEnumerator introAnimation()
    {
        introAnim.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        introAnim.GetComponent<Animator>().SetTrigger("Start");
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
    int GetTotalEnemyCount()
    {
        return waves[currentWaveIndex].NormalEnemies + waves[currentWaveIndex].TankEnemies + waves[currentWaveIndex].FastEnemies;
    }



    IEnumerator WaveHandler()
    {
        while (true)
        {
            WaveConfig currentWave;
            int totalEnemyCount;

            if (!roundBegin)
            {
                yield return new WaitForEndOfFrame();
                StartCoroutine(WaveHandler());
                break;
            }

            // Reset the car if a new round starts
            currentCarState = CarState.Resetting; // Update the state
            if (activeCarSequence != null)
            {
                StopCoroutine(activeCarSequence); // Stop any ongoing car actions
                Debug.Log("Stopped active CarSequence to prepare for the next round.");
            }

            clownCar.transform.position = startWaypoint.position;
            clownCar.transform.rotation = Quaternion.Euler(0f, 0f, 0f); // Reset orientation
            currentCarState = CarState.Idle; // Set the car back to idle
            Debug.Log("Clown car reset to the starting position for the new round.");

            startingArrows.SetActive(false);
            if (currentWaveIndex < waves.Count)
            {
                // Predefined wave
                currentWave = waves[currentWaveIndex];
                totalEnemyCount = GetTotalEnemyCount();
                Debug.Log($"Starting predefined wave {currentWaveIndex}. Total enemies: {totalEnemyCount}");
            }
            else
            {
                // Random wave
                currentWave = GenerateRandomWave(out totalEnemyCount);
                Debug.Log($"Starting random wave {currentWaveIndex}. Total enemies: {totalEnemyCount}");
            }

            
            

            // Spawn delay
            yield return new WaitForSeconds(3f);

            // Reset death count for the wave
            deathCount = 0;
            int spawnedEnemies = 0;

            // Spawn enemies (same logic as before)
            foreach (string enemyType in currentWave.SpawnOrder)
            {
                if (enemyType == "Normal")
                {
                    for (int i = 0; i < currentWave.NormalEnemies; i++)
                    {
                        yield return new WaitForSeconds(0.8f);
                        SpawnEnemy(normalEnemyPrefab,"Normal");
                        spawnedEnemies++;
                        Debug.Log($"Spawned Normal Enemy: {spawnedEnemies} spawned so far.");
                    }
                }
                else if (enemyType == "Tank")
                {
                    for (int i = 0; i < currentWave.TankEnemies; i++)
                    {
                        yield return new WaitForSeconds(1.5f);
                        SpawnEnemy(tankEnemyPrefab,"Tank");
                        spawnedEnemies++;
                        Debug.Log($"Spawned Tank Enemy: {spawnedEnemies} spawned so far.");
                    }
                }
                else if (enemyType == "Fast")
                {
                    for (int i = 0; i < currentWave.FastEnemies; i++)
                    {
                        yield return new WaitForSeconds(0.3f);
                        SpawnEnemy(fastEnemyPrefab,"Fast");
                        spawnedEnemies++;
                        Debug.Log($"Spawned Fast Enemy: {spawnedEnemies} spawned so far.");
                    }
                }
                
            }

            clownsSpawned = true; // This will signal the car to move
            // Wait for the wave to be completed
            while (deathCount < totalEnemyCount || enemyList.Count > 0)
            {
                enemyList.RemoveAll(enemy => enemy == null);
                Debug.Log($"Wave {currentWaveIndex}: Active enemies remaining: {enemyList.Count}, Death count: {deathCount}");
                yield return new WaitForSeconds(0.3f);
            }

            // Add a short delay to ensure the wave is fully complete
            yield return new WaitForSeconds(1.5f);

            // Mark the wave as complete
            
            Debug.Log("Wave complete! Signaling car to move to the finish line.");

            // Trigger the car to move to the drop-off and handle the wave
            //activeCarSequence = StartCoroutine(CarSequence());
            //if (currentCarState == CarState.Idle)
            //{

            //    Debug.Log("Started CarSequence after wave completion.");
            //}
            //else
            //{
            //    Debug.LogWarning("CarSequence not started because the car was not idle.");
            //}

            // Final cleanup
            // Reset clownsSpawned flag
            roundChangeHolder.GetComponent<Animator>().SetTrigger("Start");
            clownsSpawned = false;
            enemyList.Clear();
            Debug.Log($"Wave {currentWaveIndex} officially complete. Preparing for the next wave.");
            roundBegin = false;
            pauseButton.gameObject.SetActive(true);

            // Increment wave index and update UI
            startingArrows.SetActive(true);
            currentWaveIndex++;
            waveCount.text = $"{currentWaveIndex + 1}";
        }
    }




    WaveConfig GenerateRandomWave(out int totalEnemyCount)
    {
        WaveConfig lastWave = currentWaveIndex > 0 ? waves[Mathf.Min(currentWaveIndex - 1, waves.Count - 1)] : new WaveConfig
        {
            NormalEnemies = 8,
            TankEnemies = 0,
            FastEnemies = 0,
            SpawnOrder = new List<string> { "Normal" }
        };

        // Increase enemy counts randomly by 2-10
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

        // Calculate total enemies
        totalEnemyCount = newNormalEnemies + newTankEnemies + newFastEnemies;

        return new WaveConfig
        {
            NormalEnemies = newNormalEnemies,
            TankEnemies = newTankEnemies,
            FastEnemies = newFastEnemies,
            SpawnOrder = newSpawnOrder
        };
    }


    void SpawnEnemy(GameObject enemyPrefab, string type)
    {
        GameObject dude = Instantiate(enemyPrefab, spawnpoint.transform);
        dude.GetComponent<enmy_scrip>().dudetype = type;
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
        StartCoroutine(DamageAnimation());
        if (amount <= 0)
        {
            GameOver();
        }
    }

    IEnumerator DamageAnimation()
    {
        int IndexToShow = Random.Range(0,3);
        GameObject bleep = Instantiate(hurtSprites[IndexToShow],tentCanvas.transform);
        bleep.transform.localScale = new Vector3(30,30,30);

        int posy = Random.Range(-880, 880);
        int posx = Random.Range(-1230, 1230);
        bleep.transform.localPosition = new Vector3(posx, posy, 0);


        yield return new WaitForSeconds(1.4f);
        Destroy(bleep);
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
        /*else
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
        }*/
    }

    public void startRound()
    {
        pauseButton.gameObject.SetActive(false);
        roundBegin = true;

        // Trigger car movement
        StartCoroutine(CarSequence());
    }

    //CAR STUFFF!!!!!!!!!!!!



    IEnumerator CarSequence()
    {
        //if (currentCarState != CarState.Idle)
        //{
        //    Debug.LogWarning("CarSequence started while the car was not idle. Aborting.");
        //    yield break; // Prevent starting if the car is not idle
        //}

        currentCarState = CarState.MovingToDropOff;

        // Move the car to the drop-off point
        yield return MoveCarTo(dropOffWaypoint);
        currentCarState = CarState.Waiting;

        Debug.Log("Car parked at the drop-off. Waiting for the wave to end.");

        // Wait until the wave is marked as complete
        yield return new WaitUntil(() => clownsSpawned);

        Debug.Log("Wave complete. Car will now move to the finish line.");
        currentCarState = CarState.MovingToFinish;

        // Move the car to the finish line
        yield return MoveCarTo(finishWaypoint);

        // Reset the car for the next wave
        clownCar.transform.position = startWaypoint.position;
        clownCar.transform.rotation = Quaternion.Euler(0f, 0f, 0f); // Reset orientation
        currentCarState = CarState.Idle;

        Debug.Log("Car reset to the start for the next round.");
    }
    IEnumerator MoveCarTo(Transform targetWaypoint)
    {
        while (Vector3.Distance(clownCar.transform.position, targetWaypoint.position) > 0.1f)
        {
            // Stretch the car while it's moving
            clownCar.transform.localScale = Vector3.Lerp(
                clownCar.transform.localScale,
                new Vector3(12f, 4f, 6f), // Stretch effect (lengthened X, flattened Y, adjusted Z)
                10f * Time.deltaTime // Smooth transition for faster speed
            );

            // Move the car toward the target waypoint
            clownCar.transform.position = Vector3.MoveTowards(
                clownCar.transform.position,
                targetWaypoint.position,
                20f * Time.deltaTime // Adjusted speed for faster movement
            );

            yield return null; // Wait for the next frame
        }

        // Car reached the waypoint, compress and tip forward
        yield return CompressAndTipForward();
    }

    IEnumerator CompressAndTipForward()
    {
        Debug.Log("Car is stopping and compressing...");

        float tipDuration = 0.3f; // tipping forward time
        float resetDuration = 0.3f; // time from tiped forward to back to normal
        float elapsed = 0f;

        // Phase 1: Compress and Tip Forward
        while (elapsed < tipDuration)
        {
            // Compress the car's scale
            clownCar.transform.localScale = Vector3.Lerp(
                clownCar.transform.localScale,
                new Vector3(8f, 6f, 4f), // Compress effect (shortened X, taller Y, narrowed Z)
                elapsed / tipDuration
            );

            // Tip forward at the front pivot
            clownCar.transform.rotation = Quaternion.Lerp(
                clownCar.transform.rotation,
                Quaternion.Euler(0f, 0f, -15f), // Tilt backward (-Z direction)
                elapsed / tipDuration
            );

            elapsed += Time.deltaTime;
            yield return null; 
        }

        // Reset elapsed time for the next phase
        elapsed = 0f;

        // Phase 2: Smoothly Reset to Normal State
        while (elapsed < resetDuration)
        {
            // Reset the car's scale to normal
            clownCar.transform.localScale = Vector3.Lerp(
                clownCar.transform.localScale,
                new Vector3(10f, 5f, 5f), // Original scale
                elapsed / resetDuration
            );

            // Reset rotation to upright position
            clownCar.transform.rotation = Quaternion.Lerp(
                clownCar.transform.rotation,
                Quaternion.Euler(0f, 0f, 0f), // Upright position
                elapsed / resetDuration
            );

            elapsed += Time.deltaTime;
            yield return null; 
        }

        Debug.Log("Car reset to normal state.");
    }


    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
    }
}
