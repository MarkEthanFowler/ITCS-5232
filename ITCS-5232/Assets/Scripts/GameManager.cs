using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerManager player;

    private void Awake()
    {
        //check to see if the singleton exists
        if(instance == null)
        {
            //Create the singleton
            instance = this;

            //prevent the game object from getting destroyed
            DontDestroyOnLoad(gameObject);
        }
        else//singleton exists already destroy this game object
        {
            Destroy(gameObject);
        }
    }
}
