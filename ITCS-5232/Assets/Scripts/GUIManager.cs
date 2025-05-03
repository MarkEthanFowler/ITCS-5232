using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public static GUIManager instance;

    [SerializeField] private Image healthBarImg;
    [SerializeField] private PlayerManager player;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(player.GetCurrentHealth() <= 0)
        {
            Destroy(gameObject);
        }
    }
    #region Health Bar
    public void UpdateHealthBar(float hpPercentage)
    {
        healthBarImg.fillAmount = hpPercentage;
    }
    #endregion
}
