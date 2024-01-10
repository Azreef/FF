using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.Video;
using static UnityEngine.ParticleSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UnityEngine.Object sceneNext;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI textWin;
    [SerializeField] private GameObject panelWin;

    public VideoPlayer videoPlayer;
    public static GameManager instance;
    private System.Random random = new System.Random();

    public List<Character> players;
    public Button moveButton;
    public Button battleButton;
    public Button conquerButton;

    private bool clockwise = true;
    private int turn = 0;
    private int turnFirst;
    public int round = 0;

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
        VictoryCheck();

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
                        players[turn].drawn = false;
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
        if (players[turn].teleport == true || players[turn].move == true || players[turn].duel == true || (players[turn].tileCurrent.type == TileType.Wildcard && !players[turn].drawn))
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
            VictoryCheck();

            if (players[i].health == 0)
            {
                var tiles = FindObjectsOfType<Tile>();

                foreach (var tile in tiles)
                {
                    if (tile.owner == players[i]) tile.ResetOwner();
                }

                players[i].image.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                Destroy(players[i].gameObject);
                players.RemoveAt(i);
            }
        }

        if (turn >= players.Count) turn = 0;
    }

    public void VictoryCheck()
    {
        if (players.Count <= 1)
        {
            textWin.text = $"{players[0].gameObject.name} Wins!";
            panelWin.SetActive(true);
        }
    }

    public void PrintText(string t)
    {
        text.text = t;
    }

    public void NextScene()
    {
        SceneManager.LoadScene(sceneNext.name);
    }
}
