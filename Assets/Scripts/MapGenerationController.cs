using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapGenerationController : MonoBehaviour
{
    public static MapGenerationController instance;
    public Tilemap map;
    public RuleTile mapTile;
    public Vector2Int maxSize;
    [SerializeField] private Vector2Int minSize;
    [SerializeField] private Vector3 offset;

    public Vector2Int size;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        map.GetComponent<TilemapRenderer>().receiveShadows = true;
        map.GetComponent<TilemapRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

    //Generates random sized tilemap 
    public Vector2 SetupTilemap()
    {
        size = new Vector2Int(Random.Range(minSize.x, maxSize.x), Random.Range(minSize.y, maxSize.y));
        map.ClearAllTiles();

        Camera.main.transform.position = new Vector3((size.x  - 1) / 2f, offset.y + (size.y - 1) / 2f, offset.z - size.x / 2f);

        for (int r = 0; r < size.x; r++)
        {
            for (int c = 0; c < size.y; c++)
            {
                map.SetTile(new Vector3Int(r, c, 0), mapTile);
            }
        }

        return size;
    }
}
