using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsMenu : MonoBehaviour
{

    public string levelName;

    public void goToMainMenu()
    {
        SceneManager.LoadScene(levelName);
    }
}
