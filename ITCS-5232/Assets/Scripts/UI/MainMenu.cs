using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string levelName;

    public void StartGameMainMenu()
    {
        SceneManager.LoadScene(levelName);
    }

    public void QuitGameMainMenu()
    {
        Application.Quit();
    }
}
