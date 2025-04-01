using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseMenu : MonoBehaviour
{
    public string levelName;

    public void MainMenuLoseMenu()
    {
        SceneManager.LoadScene(levelName);
    }

    public void QuitGameLoseMenu()
    {
        Application.Quit();
    }
}
