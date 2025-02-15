using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class game_managie : MonoBehaviour
{

    private TextMeshProUGUI healthText;
    public GameObject enmy;
    public GameObject spawnpoint;
    private int enmyCount = 0;
    public float spawnDelay;
    public List<GameObject> towerList;
    public int baseTowerCost = 5;
    public TextMeshProUGUI baseTowerCostText;
    public bool anyUpgradeMenuOpen;
    public int money = 5;
    public int deathCount = 0;
    public GameObject towerPurchaseButtons;
    public TextMeshProUGUI moneyText;

    public int enemyHealth;
    // Start is called before the first frame update
    void Start()
    {
        
        healthText = GameObject.Find("healthAmount").GetComponent<TextMeshProUGUI>();
        StartCoroutine(enmy_spawner());
    }

    // Update is called once per frame
    void Update()
    {
        if (deathCount < 20)
        {
            spawnDelay = 0.7f;
            enemyHealth = 1;
        }

        else if (deathCount < 50)
        {
            spawnDelay = 0.6f;
            enemyHealth = 2;
        }

        else if (deathCount < 100)
        {
            spawnDelay = 0.5f;
            enemyHealth = 3;
        }

        else if (deathCount < 300)
        {
            spawnDelay = 0.2f;
            enemyHealth = 5;
        }

        else if (deathCount < 500)
        {
            spawnDelay = 0.05f;
            enemyHealth = 7;
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
        GameObject dude = Instantiate(enmy,spawnpoint.transform);
        dude.name = enmyCount.ToString();
        enmyCount++;
        StartCoroutine(enmy_spawner());
    }

    public void Damage()
    {
        int amount = int.Parse(healthText.text);
        amount -= 1;
        healthText.text = amount.ToString();
    }

}
