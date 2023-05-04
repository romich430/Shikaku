using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance;
    public Tilemap map;
    public RuleTile mapTile;
    public BoxCollider collider;
    private Vector3Int startPoint;
    private Vector3Int endPoint;
    private Color color = Color.white;
    private List<Vector3Int> area = new List<Vector3Int>();
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }

    public void SetupSelectionMap(Vector2 size)
    {
        collider.transform.position = new Vector3((int)size.x / 2f, (int)size.y / 2f, 0f);
        collider.size = size;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            RaycastHit hit; 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit) && hit.collider != null)
            {
                OnDrag(hit.point, Input.GetTouch(0).phase == TouchPhase.Began);
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                OnDragEnded();
            }
        }
    }

    private void OnDrag(Vector2 point, bool first = false)
    {
        Vector3Int rounded = new Vector3Int((int)Mathf.Round(point.x), (int)Mathf.Round(point.y));

        if (first)
        {
            startPoint = endPoint = rounded;
            if(GameplayManager.instance.currentTower)
                color = GameplayManager.instance.currentTower.color;
            map.SetTile(rounded, mapTile);

            map.SetTileFlags(rounded, TileFlags.None);
            map.SetColor(rounded, color);
        }
        if (endPoint != rounded)
        {
            if(GameplayManager.instance.currentTower)
                color = GameplayManager.instance.currentTower.color;
            map.ClearAllTiles();
            area = new List<Vector3Int>();
            for (int x = startPoint.x < rounded.x ? startPoint.x : rounded.x; x <= (startPoint.x > rounded.x ? startPoint.x : rounded.x); x++)
            {
                for (int y = startPoint.y < rounded.y ? startPoint.y : rounded.y; y <= (startPoint.y > rounded.y ? startPoint.y : rounded.y); y++)
                {
                    area.Add(new Vector3Int(x, y));
                    map.SetTile(new Vector3Int(x, y), mapTile);
                }   
            }
            map.SetTileFlags(rounded, TileFlags.None);
            map.SetColor(rounded, color);

            endPoint = rounded;
        }
    }

    private void OnDragEnded()
    {
        if(area.Count > 1)
            GameplayManager.instance.ProccessSelectedArea(area);
        map.ClearAllTiles();
        startPoint = default;
        endPoint = default;
        color = Color.white;
    }
}
