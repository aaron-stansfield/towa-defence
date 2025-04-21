using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class mainMenu : MonoBehaviour
{
    public GameObject introCutSceneObj;
    public GameObject loadingScreenObj;
    private bool end;
    private bool end2;


    public void Awake()
    {
       
    }

    public void Start()
    {
        if (!end || !end2)
        {
            StartCoroutine(loadingScreenOnStart());
            StartCoroutine(cutSceneTimer());
        }
    }


    private void Update()
    {
        if ((introCutSceneObj.GetComponent<VideoPlayer>().isPlaying && Input.GetMouseButton(0)) || (end && !end2))
        {
            end2 = true;
            this.transform.GetChild(0).gameObject.SetActive(true);
            loadingScreenObj.SetActive(false);
            this.GetComponent<Animator>().SetTrigger("fadeIn");
            introCutSceneObj.SetActive(false);
            
            //break;
        }
        else
        {

        }
    }


    IEnumerator cutSceneTimer()
    {
        yield return new WaitForSeconds(34);
        end = true;
    }
    IEnumerator loadingScreenOnStart()
    {
        this.GetComponent<Animator>().SetTrigger("startLoad");
        yield return new WaitForSeconds(3f);
        this.GetComponent<Animator>().StopPlayback();
        introCutSceneObj.GetComponent<VideoPlayer>().Play();
        //this.GetComponent<Animator>().SetTrigger("fadeIn");
    }

    IEnumerator beginGame()
    {
        this.GetComponent<Animator>().SetTrigger("fadeOut");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(1);
    }

    public void playGame()
    {
        // Gets the scene currently being used and finds the next scene
        // Then sets that to the active scene
        StartCoroutine(beginGame());
        //SceneManager.LoadScene(1);
    }

    public void quitGame()
    {
        // Closes game
        Application.Quit();
    }

    public void Restart()
    {
        // Restarts current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu()
    {
        // Loads main menu scene
        SceneManager.LoadScene(0);
        //somthing here about changing the pause to false to stop bug from menu
    }

    public void SwapDudes()
    {
        nothin dudescript = GameObject.Find("nothin 2 worry bout").GetComponent<nothin>();
        dudescript.cooldudes = !dudescript.cooldudes;
    }
}
