using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class BlockTowerController : MonoBehaviour
{
    public TowerBlockManager blockPrefab;
    public List<TowerBlockManager> activeBlocks;
    [SerializeField]private float spacing;
    public Material baseMaterial;
    public TextMeshPro sizeText;
    public BoxCollider collider;
    public bool isSplit = false;
    public List<Vector3Int> occupiedArea = new List<Vector3Int>();
    public Color color;

    //For specific color palettes use dictionary
    /*private Dictionary<int, Color> BasicColor = new Dictionary<int, Color>()
    {
        {0, Color.red},
        {1, Color.green},
        {2, Color.blue},
        {3, Color.yellow},
        {4, Color.magenta},
        {5, Color.cyan},
        {6, Color.gray},
        {7, Color.white},
        {8, new Color(0.75f, 0.5f, 0.25f)},
        {9, new Color(0.5f, 0.25f, 1f)},
        {10, new Color(0.25f, 0.5f, 0.25f)}
    };*/

    private int size;

    public int Size
    {
        get { return size;}
        set { size = value; SetupTower();}
    }

    private void SetupTower()
    {
        sizeText.text = size.ToString();
        Material mat = new Material(baseMaterial);
        color = /*new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f)
        );*/ Random.ColorHSV(0f, 0.5f, 1f, 1f, 0.5f, 1f); 
        mat.color = color;
        activeBlocks.First().SetupBlock(mat, new Vector3Int((int)transform.position.x, (int)transform.position.y), this, false);
        for (int i = 1; i < size; i++)
        {
            Vector3 point = new Vector3((int)activeBlocks.First().transform.position.x, (int)activeBlocks.First().transform.position.y, activeBlocks.Last().transform.position.z - spacing);
            var activeBlock = Instantiate(blockPrefab, point, Quaternion.identity, transform); 
            activeBlocks.Add(activeBlock);
            activeBlock.SetupBlock(mat, point, this);
        }

        sizeText.transform.position = new Vector3(activeBlocks.Last().transform.position.x, activeBlocks.Last().transform.position.y, activeBlocks.Last().transform.position.z - 0.12f);
    }
    
    private void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            RaycastHit hit; 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit) && hit.collider == collider)
            {
                OnDragStarted();
            }
        }
    }

    private void OnDragStarted()
    {
        GameplayManager.instance.currentTower = this;
    }

    public void Split(List<Vector3Int> area)
    {
        occupiedArea = area;
            for (int i = 1; i < activeBlocks.Count; i++)
            {
                activeBlocks[i].MoveToSpot(area[i - 1]);
                activeBlocks[i].SetColliderActive(true);
            }

            sizeText.transform.DOMove(new Vector3((int)activeBlocks.First().basePoint.x,
                (int)activeBlocks.First().basePoint.y, -0.25f), 0.3f);
            StartCoroutine(DelayAction(0.4f, () => isSplit = true));
        }

    public static IEnumerator DelayAction(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback.Invoke();
    }

    public void Fold()
    {
        if (isSplit)
        {
            foreach (var block in activeBlocks)
            {
                if (activeBlocks.First() == block) continue;
                block.MoveToSpot(block.basePoint);
                block.SetColliderActive(false);
            }

            GameplayManager.instance.OnTowerFolded(this);
            occupiedArea = new List<Vector3Int>();

            sizeText.transform.DOMove(
                new Vector3((int)activeBlocks.Last().basePoint.x, (int)activeBlocks.Last().basePoint.y,
                    activeBlocks.Last().basePoint.z - 0.12f), 0.3f);
            StartCoroutine(DelayAction(0.4f, () => isSplit = false));
        }
    }
}

public class BlockTower
{
    public Vector2 position;
    public int size;
    public BlockTowerController controller; 

    public BlockTower(Vector2 pos, int size_)
    {
        position = pos;
        size = size_;
    }
}
