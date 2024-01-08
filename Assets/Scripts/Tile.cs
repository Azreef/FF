using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public List<Tile> nextTiles;
    public TileType type = TileType.Land;

    public Character owner;
    private Material defaultMaterial;

    // Start is called before the first frame update
    void Start()
    {
        defaultMaterial = GetComponent<MeshRenderer>().material;
    }

    public void SetOwner(Character owner)
    {
        SoundManager.instance.audioSource.PlayOneShot(SoundManager.instance.tileCaptureSound);

        this.owner = owner;
        GetComponent<MeshRenderer>().material = owner.GetComponentInChildren<MeshRenderer>().material;
    }

    public void ResetOwner()
    {
        this.owner = null;
        GetComponent<MeshRenderer>().material = defaultMaterial;
    }
}

public enum TileType
{
    Start,
    Land,
    Wildcard
}
