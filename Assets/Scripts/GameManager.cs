using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
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

    private void Update()
    {
        if (players[turn].teleport == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.transform.gameObject.GetComponent<Tile>() != null)
                    {
                        players[turn].teleport = false;
                        players[turn].GoTo(hit.transform.gameObject.GetComponent<Tile>());
                        players[turn].TurnEnd();
                    }
                }
            }
        }
    }

    public void Decide()
    {
        ResetButtons();
        RemoveDeadCharacters();
        if (players[turn].teleport == true || players[turn].tileCurrent.type == TileType.Wildcard || battleButton.interactable)
            players[turn].TurnStart();
        else
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
        for (int i = players.Count - 1; i >= 0; i--)
        {
            if (players[i].health == 0)
            {
                var tiles = FindObjectsOfType<Tile>();

                foreach (var tile in tiles)
                {
                    if (tile.owner == players[i]) tile.ResetOwner();
                }

                Destroy(players[i].gameObject);

                players.RemoveAt(i);
            }
        }

        if (turn >= players.Count) turn = 0;
    }
}
