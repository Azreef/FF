using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public List<Tile> nextTiles;

    [SerializeField] private Material bruteMaterial;
    [SerializeField] private Material thiefMaterial;
    [SerializeField] private Material warriorMaterial;
    [SerializeField] private Material wizardMaterial;
    [SerializeField] private Material startMaterial;
    [SerializeField] private Material landMaterial;
    [SerializeField] private Material wildcardMaterial;

    public TileType type = TileType.Land;

    // Start is called before the first frame update
    void Start()
    {
        switch (type)
        {
            case TileType.Brute:
                break;
            case TileType.Thief:
                break;
            case TileType.Warrior:
                break;
            case TileType.Wizard:
                break;
            case TileType.Start:
                break;
            case TileType.Land:
                break;
            case TileType.Wildcard:
                break;
        }
    }
}

public enum TileType
{
    Brute,
    Thief,
    Warrior,
    Wizard,
    Start,
    Land,
    Wildcard
}
