using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject selectLevel;
    [SerializeField] Button level2Button;
    [SerializeField] Button level3Button;
    [SerializeField] TextMeshProUGUI level2Text;
    [SerializeField] TextMeshProUGUI level3Text;

    void Start()
    {
        selectLevel.SetActive(false);
    }
    void Update()
    {
        UpdateButton();
    }

    public void OpenSelectLevel()
    {
        selectLevel.SetActive(true);

    }
    public void CloseSelectLevel()
    {
        selectLevel.SetActive(false);

    }

    public void OpenLevel(int levelNo)
    {
        if (levelNo == 1)
        {
            SceneManager.LoadScene("Level 1");
        }
        else if (levelNo == 2)
        {
            SceneManager.LoadScene("Level 2");
        }
        else if (levelNo == 3)
        {
            SceneManager.LoadScene("Level 3");
        }
    }


    public void UpdateButton()
    {
        if (PlayerPrefs.GetInt("level2Purchased") == 1)
        {
            level2Button.enabled= true;
            level2Text.text = "Level 2";
        }
        else if(PlayerPrefs.GetInt("level2Purchased") == 0)
        {
            level2Button.enabled = false;
            level2Text.text = "Locked";
        }


        if (PlayerPrefs.GetInt("level3Purchased") == 1)
        {
            level3Button.enabled = true;
            level3Text.text = "Level 3";
        }
        else if (PlayerPrefs.GetInt("level3Purchased") == 0)
        {
            level3Button.enabled = false;
            level3Text.text = "Locked";
        }
    }
}
