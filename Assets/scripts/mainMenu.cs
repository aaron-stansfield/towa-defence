using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    public void playGame()
    {
        // Gets the scene currently being used and finds the next scene
        // Then sets that to the active scene
        
        SceneManager.LoadScene(1);
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
