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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
    }
}
