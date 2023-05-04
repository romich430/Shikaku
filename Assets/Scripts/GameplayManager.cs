using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance;
    [HideInInspector]public BlockTowerController currentTower;
    private LevelData currentLevelData;
    public TextMeshProUGUI levelText;
    private int level = 1;
    [SerializeField] private AudioSource audio;
    [SerializeField] private ParticleSystem particle;
    private bool isHard;
    public bool IsHard 
    {
        get { return isHard; }
    }

    public LevelData CurrentLevelData
    {
        get;
        set;
    }
    
    //To track if colors of towers arent repeating, use with color palettes
    //public List<Color> usedColors = new List<Color>();
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }
    
    public void StartGameplay()
    {
        if(currentLevelData != null)ClearAll();
        Vector2 size = MapGenerationController.instance.SetupTilemap();
        SelectionManager.instance.SetupSelectionMap(size);

        //if level is failed to generate, running command again until positive result
        LevelData result = null;
        while (result == null)
            result = LevelGeneratorController.instance.GenerateNewLevel(size);
        currentLevelData = result;
    }

    public void SetDifficulty(bool isHard_)
    {
        isHard = isHard_;
    }

    public void ProccessSelectedArea(List<Vector3Int> area)
    {
        if (!currentTower) return;
        if (currentTower.Size != area.Count) return;

        area.Remove(new Vector3Int((int)currentTower.activeBlocks.First().basePoint.x, (int)currentTower.activeBlocks.First().basePoint.y));
        foreach (var point in area)
        {
            Debug.Log(currentLevelData.isFilled[point.x][point.y]);
            if (currentLevelData.isFilled[point.x][point.y])
            {
                Debug.Log("Area contains another block!");
                //tell the tower that occupied that place to remove tiles
                return;
            }

            currentLevelData.isFilled[point.x][point.y] = true;
        }
        
        if (currentTower.isSplit)
        {
            StartCoroutine(BlockTowerController.DelayAction(0.7f, () =>
            {
                currentTower.Split(area);
                currentTower = null;
            }));
        }
        else
        {
            currentTower.Split(area);
            currentTower = null;
        }

        if (CheckIfCompleted())
        {
            particle.Play();
            audio.Play();
            IncreaseDifficulty();
            StartCoroutine(BlockTowerController.DelayAction(0.5f, StartGameplay));
        }
    }

    private void IncreaseDifficulty()
    {
        level++;
        levelText.text = "Level " + level;
        if(MapGenerationController.instance.maxSize.y != 9) 
            MapGenerationController.instance.maxSize = new Vector2Int(MapGenerationController.instance.maxSize.x + 1, MapGenerationController.instance.maxSize.y + 1);
        if(LevelGeneratorController.instance.maxTowerHeight != 8)
            LevelGeneratorController.instance.maxTowerHeight++;
    }

    public void OnTowerFolded(BlockTowerController towerController)
    {
        foreach (var point in towerController.occupiedArea)
        {
            currentLevelData.isFilled[point.x][point.y] = false;
        }
    }

    private bool CheckIfCompleted()
    {
        foreach (var row in currentLevelData?.isFilled)
        {
            if (row.Contains(false))
            {
                return false;
            }
        }

        return true;
    }

    private void ClearAll()
    {
        foreach (var tower in currentLevelData?.towers)
        {
            Destroy(tower?.controller.gameObject);
        }
    }
}
