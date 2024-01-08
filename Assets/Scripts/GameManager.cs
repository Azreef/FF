using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private System.Random random = new System.Random();

    public List<Character> players;
    public Button moveButton;
    public Button battleButton;
    public Button conquerButton;

    private bool clockwise = true;
    private int turn = 0;
    private int turnFirst;
    private int round = 0;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        clockwise = (random.Next(2) == 0);
        turn = turnFirst = 0;//turn = turnFirst = random.Next(players.Count - 1);

        players[turn].TurnStart();
    }

    public void Decide()
    {
        if (battleButton.interactable || conquerButton.interactable)
        {
            ResetButtons();
            players[turn].TurnStart();
            return;
        }

        ResetButtons();
        RemoveDeadCharacters();
        TurnNext();
    }

    public void TurnNext()
    {
        turn++;// turn += clockwise ? 1 : -1;
        if (turn >= players.Count) turn = 0;
        //if (turn <= -1) turn = players.Count - 1;

        if (turn == turnFirst) round++;

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

    public void RemoveDeadCharacters()
    {
        foreach(Character character in players)
        {
            if(character.health == 0)
            {
                Destroy(character.gameObject);

                if (turn >= players.Count) turn = 0;
            }
        }
    }
}
