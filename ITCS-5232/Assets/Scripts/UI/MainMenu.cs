using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string levelName;
    public string settingsName;

    public void StartGameMainMenu()
    {
        SceneManager.LoadScene(levelName);
    }

    public void goToControls()
    {
        SceneManager.LoadScene(settingsName);
    }

    public void QuitGameMainMenu()
    {
        Application.Quit();
    }
}
