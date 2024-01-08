using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Role role;
    [SerializeField] private GameObject brute;
    [SerializeField] private GameObject thief;
    [SerializeField] private GameObject warrior;
    [SerializeField] private GameObject wizard;
    [SerializeField] private GameObject startTile;

    private int health;
    private int healthMax;
    private int healthDie;
    private int movementDie;
    private int attackDie;
    private Tile currentTile;
    private Tile lastTile;
    
    private void Awake()
    {

    }

    private void OnDestroy()
    {
        
    }

    void Start()
    {
        currentTile = startTile.GetComponent<Tile>();

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

        int movementRoll = Roll(movementDie);

        for (int i = 0; i < movementRoll; i++)
        {
            SoundManager.instance.audioSource.PlayOneShot(SoundManager.instance.pawnMoveSound);

            int nt;

            do
            {
                nt = UnityEngine.Random.Range(0, currentTile.nextTiles.Count);
            } while (lastTile == currentTile.nextTiles[nt]);

            if (Vector3.Distance(transform.position, currentTile.nextTiles[nt].transform.position + new Vector3(0, 4, 0)) > 5)
                SoundManager.instance.audioSource.PlayOneShot(SoundManager.instance.portalSound);

            transform.position = currentTile.nextTiles[nt].transform.position + new Vector3(0, 1, 0);
            lastTile = currentTile;
            currentTile = currentTile.nextTiles[nt];

            if (currentTile.type == TileType.Start) Heal(2);
        }
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
