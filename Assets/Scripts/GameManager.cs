using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    //private System.Random random = new System.Random();

    public List<Character> players;
    public Button moveButton;
    public Button battleButton;
    public Button conquerButton;

    private int turn = 0;
    private int round = 0;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //int turn = random.Next(players.Count - 1);
        players[0].TurnStart();
    }

    public void Decide()
    {
        ResetButtons();
        TurnNext();
    }

    public void TurnNext()
    {
        turn++; //turn += clockwise ? 1 : -1;
        if (turn >= players.Count) turn = 0;
        //if (turn <= -1) turn = players.Count - 1;

        players[turn].TurnStart();
    }

    public void ResetButtons()
    {
        moveButton.interactable = false;
        battleButton.interactable = false;
        conquerButton.interactable = false;

        moveButton.onClick.RemoveAllListeners();
        battleButton.onClick.RemoveAllListeners();
        conquerButton.onClick.RemoveAllListeners();
    }
}
