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
    public GameObject activeTurret;
    // Start is called before the first frame update
    void Start()
    {
        healthText = GameObject.Find("healthAmount").GetComponent<TextMeshProUGUI>();
        StartCoroutine(enmy_spawner());
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    IEnumerator enmy_spawner()
    {
        yield return new WaitForSeconds(1);
        Instantiate(enmy,spawnpoint.transform);
        StartCoroutine(enmy_spawner());
    }

    public void Damage()
    {
        int amount = int.Parse(healthText.text);
        amount -= 1;
        healthText.text = amount.ToString();
    }
}
