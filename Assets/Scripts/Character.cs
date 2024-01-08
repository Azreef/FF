using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Character : MonoBehaviour
{
    [SerializeField] private Role role;
    [SerializeField] private GameObject brute;
    [SerializeField] private GameObject thief;
    [SerializeField] private GameObject warrior;
    [SerializeField] private GameObject wizard;
    [SerializeField] private GameObject tileStart;

    private int health;
    private int healthMax;
    private int healthDie;
    private int movementDie;
    private int attackDie;
    private Tile tileCurrent;
    private Tile tileLast;
    private TileType tt;
    
    private void Awake()
    {

    }

    private void OnDestroy()
    {
        GameManager.instance.players.Remove(this);
    }

    void Start()
    {
        tileLast = tileCurrent = tileStart.GetComponent<Tile>();
        tt = tileCurrent.type;
        

        switch(role)
        {
            case Role.Brute:
                health = healthMax = 20;
                healthDie = 8;
                movementDie = 8;
                attackDie = 1;
                Instantiate(brute, this.transform);
                break;
            case Role.Thief:
                health = healthMax = 20;
                healthDie = 8;
                movementDie = 8;
                attackDie = 1;
                Instantiate(thief, this.transform);
                break;
            case Role.Warrior:
                health = healthMax = 20;
                healthDie = 8;
                movementDie = 8;
                attackDie = 1;
                Instantiate(warrior, this.transform);
                break;
            case Role.Wizard:
                health = healthMax = 20;
                healthDie = 8;
                movementDie = 8;
                attackDie = 1;
                Instantiate(wizard, this.transform);
                break;
        }
    }

    public void TurnStart()
    {
        GameManager.instance.moveButton.interactable = true;

        switch (tt)
        {
            case TileType.Brute:
                if (role == Role.Brute) break;
                goto default;
            case TileType.Thief:
                if (role == Role.Thief) break;
                goto default;
            case TileType.Warrior:
                if (role == Role.Warrior) break;
                goto default;
            case TileType.Wizard:
                if (role == Role.Wizard) break;
                goto default;
            case TileType.Start:
            case TileType.Wildcard:
            case TileType.Land:
                break;
            default:
                if (health > 5) GameManager.instance.moveButton.interactable = false;
                GameManager.instance.battleButton.interactable = true;
                GameManager.instance.conquerButton.interactable = true;
                break;
        }
        GameManager.instance.moveButton.onClick.AddListener(Move);
        GameManager.instance.battleButton.onClick.AddListener(Battle);
        GameManager.instance.conquerButton.onClick.AddListener(Conquer);
    }

    public void TurnEnd()
    {
        GameManager.instance.Decide();
    }

    public void Heal(int amount)
    {
        SoundManager.instance.audioSource.PlayOneShot(SoundManager.instance.pawnHealSound);

        health = Math.Min(healthMax, health + amount);
    }

    public void Damage(int amount)
    {
        health = Math.Max(0, health - amount);
    }

    public void Move()
    {
        SoundManager.instance.audioSource.PlayOneShot(SoundManager.instance.inGameButtonSound);
        GameManager.instance.moveButton.interactable = false;

        StartCoroutine(MoveCoroutine());
    }

    protected IEnumerator MoveCoroutine()
    {

        int movementRoll = Roll(movementDie);

        for (int i = 0; i < movementRoll; i++)
        {
            SoundManager.instance.audioSource.PlayOneShot(SoundManager.instance.pawnMoveSound);

            int nt;

            do
            {
                nt = UnityEngine.Random.Range(0, tileCurrent.nextTiles.Count);
            } while (tileLast == tileCurrent.nextTiles[nt]);

            if (Vector3.Distance(transform.position, tileCurrent.nextTiles[nt].transform.position + new Vector3(0, 1, 0)) > 15)
                SoundManager.instance.audioSource.PlayOneShot(SoundManager.instance.portalSound);

            transform.position = tileCurrent.nextTiles[nt].transform.position + new Vector3(0, 1, 0);
            tileLast = tileCurrent;
            tileCurrent = tileCurrent.nextTiles[nt];

            if (tileCurrent.type == TileType.Start) Heal(2);

            yield return new WaitForSeconds(.5f);
        }

        TurnEnd();
    }

    public void Battle()
    {
        SoundManager.instance.audioSource.PlayOneShot(SoundManager.instance.inGameButtonSound);

    }

    public void Conquer()
    {
        SoundManager.instance.audioSource.PlayOneShot(SoundManager.instance.inGameButtonSound);

    }

    private int Roll(int face)
    {
        int r = UnityEngine.Random.Range(1, face + 1);
        Debug.Log(string.Format("Rolling 1d{0}: {1}", face, r));
        return r;
    }
}

enum Role
{
    Brute,
    Thief,
    Warrior,
    Wizard
}
