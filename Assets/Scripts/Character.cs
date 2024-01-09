using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [SerializeField] private Role role;
    [SerializeField] private GameObject brute;
    [SerializeField] private GameObject thief;
    [SerializeField] private GameObject warrior;
    [SerializeField] private GameObject wizard;
    [SerializeField] private GameObject tileStart;
    [SerializeField] private Image image;
    [SerializeField] private Image healthBar;

    private System.Random random = new System.Random();
    private int healthMax;
    private int healthDie;
    private int movementDie;
    private int attack;
    public Tile tileCurrent;
    private Tile tileLast;

    public int health;

    private bool invulnerability = false;
    private bool damageBoost = false;
    private bool speedBoost = false;

    public bool teleport = false;
    public bool duel = false;
    public bool drawn = false;
    public bool move = false;

    void Start()
    {
        tileLast = tileCurrent = tileStart.GetComponent<Tile>();

        switch(role)
        {
            case Role.Brute:
                health = healthMax = 28;
                healthDie = 12;
                movementDie = 4;
                attack = 1;
                Instantiate(brute, this.transform);
                break;
            case Role.Thief:
                health = healthMax = 16;
                healthDie = 6;
                movementDie = 12;
                attack = 1;
                Instantiate(thief, this.transform);
                break;
            case Role.Warrior:
                health = healthMax = 20;
                healthDie = 8;
                movementDie = 8;
                attack = 1;
                Instantiate(warrior, this.transform);
                break;
            case Role.Wizard:
                health = healthMax = 16;
                healthDie = 6;
                movementDie = 8;
                attack = 2;
                Instantiate(wizard, this.transform);
                break;
        }
    }

    private void Update()
    {

        healthBar.fillAmount = ((float)health / (float)healthMax);
    }

    public void TurnStart()
    {
        image.transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);

        if (teleport == true && !drawn)
            return;

        if (tileCurrent?.type == TileType.Wildcard && !drawn)
        {
            Draw();
            if (move != true) return;
        }

        GameManager.instance.moveButton.onClick.AddListener(Move);
        GameManager.instance.battleButton.onClick.AddListener(Battle);
        GameManager.instance.conquerButton.onClick.AddListener(Conquer);

        GameManager.instance.moveButton.interactable = true;

        /// TODO
        /// Disable Battle/Conquer when attacker on defender are on the other's tile

        if (tileCurrent?.type == TileType.Land && tileCurrent?.owner != this && tileCurrent?.owner != null)
        {
            if (health > 5) GameManager.instance.moveButton.interactable = false;
            GameManager.instance.battleButton.interactable = true;
            GameManager.instance.conquerButton.interactable = true;
        }
    }

    public void TurnEnd()
    {
        image.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
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

        int movementRoll = Roll(movementDie) + ((speedBoost) ? 2: 0);

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

        speedBoost = false;
        drawn = false;
        move = false;

        LandingAction();

        TurnEnd();
    }

    public void LandingAction()
    {
        if (tileCurrent.type == TileType.Land)
        {
            if (tileCurrent.owner == null)
            {
                tileCurrent.SetOwner(this);
            }
            else if (tileCurrent.owner != this && GameManager.instance.round > 0)
            {
                duel = true;
            }
        }
    }

    public void Battle()
    {
        SoundManager.instance.audioSource.PlayOneShot(SoundManager.instance.inGameButtonSound);
        GameManager.instance.ResetButtons();

        StartCoroutine(BattleCoroutine());
    }

    protected IEnumerator BattleCoroutine()
    {
        if (tileCurrent?.owner == null)
        {
            TurnEnd();
        }

        int attackerHealthRoll = Roll(healthDie);

        int defenderHealthRoll = Roll(tileCurrent.owner.healthDie);
        int defenderMovementRoll = Roll(tileCurrent.owner.movementDie);
        int defenderTotal = defenderHealthRoll + defenderMovementRoll;

        Debug.Log($"Attacker: {role} | Defender: {tileCurrent.owner}");
        Debug.Log($"Attacker rolled {attackerHealthRoll} | Health Roll: {attackerHealthRoll}");
        Debug.Log($"Defender rolled {defenderTotal} | Health Roll: {defenderHealthRoll} | Movement Roll: {defenderMovementRoll}");

        if (attackerHealthRoll > defenderTotal)
        {
            tileCurrent.owner.Damage(tileCurrent.owner.invulnerability ? 0 : damageBoost ? attack * 2 : attack);
            Debug.Log($"{role} won!");
        }
        else
        {
            Debug.Log($"{tileCurrent.owner} won!");
        }

        Damage(invulnerability ? 0 : tileCurrent.owner.damageBoost ? tileCurrent.owner.attack * 2 : tileCurrent.owner.attack);
        tileCurrent.owner.Damage(tileCurrent.owner.invulnerability ? 0 : damageBoost ? attack * 2 : attack);

        yield return new WaitForSeconds(.25f);

        invulnerability = false;
        damageBoost = false;
        duel = false;

        TurnEnd();
    }

    public void Conquer()
    {
        SoundManager.instance.audioSource.PlayOneShot(SoundManager.instance.inGameButtonSound);
        GameManager.instance.ResetButtons();

        StartCoroutine(ConquerCoroutine());
    }

    protected IEnumerator ConquerCoroutine()
    {
        int win = 0;

        if(tileCurrent?.owner == null)
        {
            TurnEnd();
        }

        for (int i = 1; i < 4; i++)
        {
            int attackerHealthRoll = Roll(healthDie);
            int attackerMovementRoll = Roll(movementDie);
            int attackerTotal = attackerHealthRoll + attackerMovementRoll;

            int defenderHealthRoll = Roll(tileCurrent.owner.healthDie);
            int defenderMovementRoll = Roll(tileCurrent.owner.movementDie);
            int defenderTotal = defenderHealthRoll + defenderMovementRoll;

            Debug.Log($"Attacker: {role} | Defender: {tileCurrent.owner}");
            Debug.Log($"Attacker rolled {attackerTotal} | Health Roll: {attackerHealthRoll} | Movement Roll: {attackerMovementRoll}");
            Debug.Log($"Defender rolled {defenderTotal} | Health Roll: {defenderHealthRoll} | Movement Roll: {defenderMovementRoll}");

            if (attackerTotal > defenderTotal)
            {
                tileCurrent.owner.Damage(tileCurrent.owner.invulnerability ? 0 : damageBoost ? attack * 2 : attack);
                Debug.Log($"{role} won {i}/3!"); win++;
            }
            else
            {
                Damage(tileCurrent.owner.attack);
                Debug.Log($"{tileCurrent.owner} won {i}/3!");
            }

            Damage(invulnerability ? 0 : tileCurrent.owner.damageBoost ? tileCurrent.owner.attack * 2 : tileCurrent.owner.attack);
            tileCurrent.owner.Damage(tileCurrent.owner.invulnerability ? 0 : damageBoost ? attack * 2 : attack);

            yield return new WaitForSeconds(.25f);
        }

        Debug.Log($"{role} won {win} times!");

        if (win >= 2) tileCurrent.SetOwner(this);

        invulnerability = false;
        damageBoost = false;
        duel = false;

        TurnEnd();
    }

    private void Draw()
    {
        int selected = random.Next(16);

        Debug.Log("WILDCARD:");

        switch (selected)
        {
            case 0:
                Debug.Log("YOU FOUND A HEALING POTION, HEAL 2 HEALTH POINT");
                Heal(2);
                break;
            case 1:
                Debug.Log("MOVE TO THE NEAREST ENEMY AND DUEL");
                GoToNearestEnemy();
                break;
            case 2:
                Debug.Log("GO TO START AND RECOVER TWO HEALTH POINTS");
                GoToStart();
                Heal(2);
                break;
            case 3:
                Debug.Log("MOVE BACKWARD 2 SQUARE SPACE");
                MoveBack(2);
                break;
            case 4:
                Debug.Log("SWOOOOSH, EVERY PLAYER HEAL 1 HEALTH POINT");
                HealAll();
                break;
            case 5:
                Debug.Log("MOVE TO SQUARE 17");
                GoToTile(17);
                break;
            case 6:
                Debug.Log("MOVE FORWARD 2 SQUARE SPACE");
                GoTo(tileCurrent.nextTiles[0].nextTiles[0]);
                break;
            case 7:
                Debug.Log("THROW MOVE DICE AND MOVE");
                move = true;
                drawn = true;
                return;
            case 8:
                Debug.Log("MOVE TO YOUR NEAREST OWNED PLACE");
                GoToNearestTerritory();
                break;
            case 9:
                Debug.Log("YOUR NEXT BATTLE WILL NOT TAKE DAMAGE");
                invulnerability = true;
                break;
            case 10:
                Debug.Log("TELEPORT TO ANYWHERE YOU DESIRE");
                teleport = true;
                drawn = true;
                return;
            case 11:
                Debug.Log("YOU STEPPED ON A SEA URCHIN, GET HIT 1 DAMAGE");
                Damage(1);
                break;
            case 12:
                Debug.Log("YOUR NEXT BATTLE DEALS DOUBLE DAMAGE");
                damageBoost = true;
                break;
            case 13:
                Debug.Log("MOVE TO SQUARE 20");
                GoToTile(20);
                break;
            case 14:
                Debug.Log("BOOTS OF SPEED, YOUR NEXT MOVE WILL ADD 2");
                speedBoost = true;
                break;
            case 15:
                Debug.Log("FOUND A TOWN PORTAL, GO TO START AND HEAL 4 HEALTH POINT");
                GoToStart();
                Heal(4);
                break;
            default:
                throw new NotImplementedException();
        }

        drawn = true;
        TurnEnd();
    }

    public void HealAll()
    {
        var ps = GameManager.instance.players;
        foreach (Character character in ps)
        {
            health = Math.Min(healthMax, health + 1);
        }
    }

    private void MoveBack(int v)
    {
        var tiles = FindObjectsOfType<Tile>();

        foreach (var tile in tiles)
        {
            Debug.Log(tile.nextTiles[0].nextTiles[0]);
            if (tile.nextTiles[0].nextTiles[0] == tileCurrent)
            {
                transform.position = tile.transform.position + new Vector3(0, 1, 0);
                tileLast = tileCurrent;
                tileCurrent = tile;
                break;
            }
        }
    }

    public void GoTo(Tile tile)
    {
        transform.position = tile.transform.position + new Vector3(0, 1, 0);
        tileLast = tileCurrent;
        tileCurrent = tile;

        LandingAction();
    }

    private void GoToTile(int destination)
    {
        var tiles = FindObjectsOfType<Tile>().OrderBy(p => p.name).ToList();
        destination = Math.Min(destination, tiles.Count() - 1);

        GoTo(tiles[destination]);
    }

    private void GoToStart()
    {
        GoTo(tileStart.GetComponent<Tile>());
    }

    private void GoToNearestTerritory()
    {
        var tiles = FindObjectsOfType<Tile>();

        Tile tileDestination = tileCurrent;

        float distance = 1000.0f;

        foreach (var tile in tiles)
        {
            if(tile.owner == this && Vector3.Distance(tileCurrent.transform.position, tile.transform.position) < distance)
            {
                tileDestination = tile;
                distance = Vector3.Distance(tileCurrent.transform.position, tile.transform.position);
            }
        }

        GoTo(tileDestination);
    }

    private void GoToNearestEnemy()
    {
        var tiles = FindObjectsOfType<Tile>();

        Tile tileDestination = tileCurrent;

        float distance = 1000.0f;

        foreach (var tile in tiles)
        {
           if (tile.type == TileType.Land && tile.owner != null && tile.owner != this && Vector3.Distance(tileCurrent.transform.position, tile.transform.position) < distance)
            {
                tileDestination = tile;
                distance = Vector3.Distance(tileCurrent.transform.position, tile.transform.position);
            }
        }

        GoTo(tileDestination);
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
