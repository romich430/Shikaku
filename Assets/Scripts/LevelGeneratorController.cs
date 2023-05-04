using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGeneratorController : MonoBehaviour
{
    public enum Shape
    {
        HorizontalLine,
        VerticalLine,
        HorizontalRect,
        VerticalRect
    };
    public static LevelGeneratorController instance;
    public BlockTowerController towerPrefab;
    public int maxTowerHeight = 3;
    public LevelData currentlevelData;
    
    private int area;
    private Shape shape;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }

    public LevelData GenerateNewLevel(Vector2 size)
    {
        currentlevelData = new LevelData(size);
        area = 0;

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                if (!currentlevelData.isFilled[x][y])
                {
                    int X = x;
                    int Y = y;
                    if (area == 0)
                    {
                        if (GetRandomAreaAndShape(new Vector2Int(x, y)) == 1)
                        {
                            return null;
                        }
                    }

                    for (int i = 0; i < area; i++)
                    {
                        if (shape == Shape.HorizontalLine)
                            currentlevelData.isFilled[X + i][Y] = true;
                        else if (shape == Shape.VerticalLine)
                            currentlevelData.isFilled[X][Y + i] = true;
                        else if (shape == Shape.HorizontalRect)
                        {
                            currentlevelData.isFilled[X + i][Y] = true;
                            currentlevelData.isFilled[X + i][Y + 1] = true;
                            area--;
                        }
                        else if (shape == Shape.VerticalRect)
                        {
                            currentlevelData.isFilled[X][Y + i] = true;
                            currentlevelData.isFilled[X + 1][Y + i] = true;
                            area--;
                        }
                    }

                    area = 0;
                }
            }
        }
        
        PlaceTowers();
        return ConvertData();
    }

    private LevelData ConvertData()
    {
        return new LevelData(currentlevelData);
    }

    private int GetRandomAreaAndShape(Vector2Int cell)
    {
        int spaceX = 0;
        for (int i = cell.x; i < currentlevelData.size.x; i++)
        {
            if (!currentlevelData.isFilled[i][cell.y])
            {
                spaceX++;
            }
            else break;
        }
        int spaceY = 0;
        for (int i = cell.y; i < currentlevelData.size.y; i++)
        {
            if (!currentlevelData.isFilled[cell.x][i])
            {
                spaceY++;
            }
            else break;
        }

        if (spaceX == 1 && spaceY == 1)
        {
            return 1;
        }

        shape = GetShape(spaceX, spaceY);
        area = GetArea(spaceX, spaceY);
        
        if(GameplayManager.instance.IsHard)
            currentlevelData.towers.Add(new BlockTower(GetRandomPosForTower(cell), area));
        else
            currentlevelData.towers.Add(new BlockTower(cell, area));
        return 0;
    }

    private Vector2 GetRandomPosForTower(Vector2Int cell)
    {
        //Debug.Log("Input point: " + cell + ", area = " + area);
        if (Random.Range(0, 100) > 50)
            return cell;

        int a = area;
        Vector2Int c = cell;
        switch (shape)
        {
            case Shape.HorizontalLine:
                c = new Vector2Int(c.x + a - 1, c.y);
                //Debug.Log("HLine Output point: " + c);
                break;
            case Shape.VerticalLine:
                c = new Vector2Int(c.x, c.y + a - 1);
                //Debug.Log("VLine Output point: " + c);
                break;
            case Shape.HorizontalRect:
                c = new Vector2Int(c.x + a/2 - 1, c.y + 1);
                //Debug.Log("HRect Output point: " + c);
                break;
            case Shape.VerticalRect:
                c = new Vector2Int(c.x + 1, c.y + a/2 - 1);
                //Debug.Log("VRect Output point: " + c);
                break;
        }

        return c;
    }

    private Shape GetShape(int spaceX, int spaceY)
    {
        if (spaceX == 1)
            return Shape.VerticalLine;
        if (spaceY == 1)
            return Shape.HorizontalLine;
        return (Shape)Random.Range(0, (spaceX > 1 && spaceY > 1) ? 3 : 2);
    }

    private int GetArea(int spaceX, int spaceY)
    {
        switch (shape)
        {
            case Shape.HorizontalLine:
                return Random.Range(2, spaceX > maxTowerHeight ? maxTowerHeight : spaceX);
            case Shape.VerticalLine:
                return Random.Range(2, spaceY > maxTowerHeight ? maxTowerHeight : spaceY);
            case Shape.HorizontalRect:
                int a = Random.Range(2, spaceX > maxTowerHeight / 2 ? maxTowerHeight : spaceX * 2);
                return a % 2 == 0 ? a : a - 1;
            case Shape.VerticalRect:
                a = Random.Range(2, spaceY > maxTowerHeight / 2 ? maxTowerHeight : spaceY * 2);
                return a % 2 == 0 ? a : a - 1;
        }

        return 0;
    }
    
    private void PlaceTowers()
    {
        foreach (var tower in currentlevelData.towers)
        {
            tower.controller = Instantiate(towerPrefab, tower.position, quaternion.identity);
            tower.controller.Size = tower.size;
        }
    }
}

public class LevelData
{
    public List<List<bool>> isFilled = new List<List<bool>>();
    public Vector2 size;
    public List<BlockTower> towers;
    
    public LevelData(Vector2 size_)
    {
        size = size_;
        towers = new List<BlockTower>();
        for (int x = 0; x < size.x; x++)
        {
            isFilled.Add(new List<bool>());
            for (int y = 0; y < size.y; y++)
            {
                isFilled[x].Add(false);
            }
        }
    }

    public LevelData(LevelData data)
    {
        size = data.size;
        towers = data.towers;
        for (int x = 0; x < size.x; x++)
        {
            isFilled.Add(new List<bool>());
            for (int y = 0; y < size.y; y++)
            {
                isFilled[x].Add(false);
            }
        }

        foreach (var tower in towers)
        {
            isFilled[(int)tower.position.x][(int)tower.position.y] = true;
        }
    }
}