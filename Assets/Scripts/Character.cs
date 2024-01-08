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
        

        switch(role)
        {
            case Role.Brute:
                health = healthMax = 28;
                healthDie = 12;
                movementDie = 4;
                attackDie = 1;
                Instantiate(brute, this.transform);
                break;
            case Role.Thief:
                health = healthMax = 16;
                healthDie = 6;
                movementDie = 12;
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
                health = healthMax = 16;
                healthDie = 6;
                movementDie = 8;
                attackDie = 2;
                Instantiate(wizard, this.transform);
                break;
        }
    }

    public void TurnStart()
    {
        GameManager.instance.moveButton.interactable = true;

        switch (tileCurrent?.type)
        {
            case TileType.Start:
            case TileType.Wildcard:
                break;
            case TileType.Land:
                if (tileCurrent.owner == this) break;
                goto default;
            default:
                if (tileCurrent == null) break;
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

            if (Vector3.Distance(transform.position, tileCurrent.nextTiles[nt].transform.position + new Vector3(0, 1, 0)) > 10)
                SoundManager.instance.audioSource.PlayOneShot(SoundManager.instance.portalSound);

            transform.position = tileCurrent.nextTiles[nt].transform.position + new Vector3(0, 1, 0);
            tileLast = tileCurrent;
            tileCurrent = tileCurrent.nextTiles[nt];

            if (tileCurrent.type == TileType.Start) Heal(2);

            yield return new WaitForSeconds(.1f);
        }

        if (tileCurrent.type == TileType.Land && tileCurrent.owner == null) tileCurrent.SetOwner(this);

        if (tileCurrent.type == TileType.Land && tileCurrent.owner != this)
        {
            GameManager.instance.battleButton.interactable = true;
            GameManager.instance.conquerButton.interactable = true;
            yield break;
        }

        TurnEnd();
    }

    public void Battle()
    {
        SoundManager.instance.audioSource.PlayOneShot(SoundManager.instance.inGameButtonSound);
        GameManager.instance.battleButton.interactable = false;
        GameManager.instance.conquerButton.interactable = false;

        StartCoroutine(BattleCoroutine());
    }

    protected IEnumerator BattleCoroutine()
    {
        int attackerHealthRoll = Roll(healthDie);

        int defenderHealthRoll = Roll(tileCurrent.owner.healthDie);
        int defenderMovementRoll = Roll(tileCurrent.owner.movementDie);
        int defenderAttackRoll = Roll(tileCurrent.owner.attackDie);
        int defenderTotal = defenderHealthRoll + defenderMovementRoll + defenderAttackRoll;

        Debug.Log($"Attacker: {role} | Defender: {tileCurrent.owner}");
        Debug.Log($"Attacker rolled {attackerHealthRoll} | Health Roll: {attackerHealthRoll}");
        Debug.Log($"Defender rolled {defenderTotal} | Health Roll: {defenderHealthRoll} | Movement Roll: {defenderMovementRoll} | Attack Roll: {defenderAttackRoll}");

        if (attackerHealthRoll > defenderTotal)
        {
            tileCurrent.SetOwner(this);
            Debug.Log($"{role} won!");
        }
        else
        {
            Debug.Log($"{tileCurrent.owner} won!");
        }

        yield return new WaitForSeconds(.25f);

        TurnEnd();
    }

    public void Conquer()
    {
        SoundManager.instance.audioSource.PlayOneShot(SoundManager.instance.inGameButtonSound);
        GameManager.instance.conquerButton.interactable = false;
        GameManager.instance.battleButton.interactable = false;

        StartCoroutine(ConquerCoroutine());
    }

    protected IEnumerator ConquerCoroutine()
    {
        int win = 0;

        for (int i = 1; i < 4; i++)
        {
            int attackerHealthRoll = Roll(healthDie);
            int attackerMovementRoll = Roll(movementDie);
            int attackerTotal = attackerHealthRoll + attackerMovementRoll;

            int defenderHealthRoll = Roll(tileCurrent.owner.healthDie);
            int defenderMovementRoll = Roll(tileCurrent.owner.movementDie);
            int defenderAttackRoll = Roll(tileCurrent.owner.attackDie);
            int defenderTotal = defenderHealthRoll + defenderMovementRoll + defenderAttackRoll;

            Debug.Log($"Attacker: {role} | Defender: {tileCurrent.owner}");
            Debug.Log($"Attacker rolled {attackerTotal} | Health Roll: {attackerHealthRoll} | Movement Roll: {attackerMovementRoll}");
            Debug.Log($"Defender rolled {defenderTotal} | Health Roll: {defenderHealthRoll} | Movement Roll: {defenderMovementRoll} | Attack Roll: {defenderAttackRoll}");

            if (attackerTotal > defenderTotal)
            {
                Debug.Log($"{role} won {i}/3!"); win++;
            }
            else
                Debug.Log($"{tileCurrent.owner} won {i}/3!");

            yield return new WaitForSeconds(.25f);
        }

        Debug.Log($"{role} won {win} times!");

        if (win >= 2) tileCurrent.SetOwner(this);

        TurnEnd();
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
