using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public MyBlockController Current { get; set; }
    public float GameSpeed => gameSpeed;
    [SerializeField, Range(0.1f, 1f)] private float gameSpeed = 1f;

    private const int GridSizeX = 10;
    private const int GridSizeY = 20;
    public bool[,] Grid = new bool[GridSizeX, GridSizeY];

    [SerializeField] private List<MyBlockController> listPrefabs;
    private List<MyBlockController> listHistory = new List<MyBlockController>();
    
    #region Test

    public bool IsOpenTest;

    [SerializeField] private SpriteRenderer displayDataPrefabs;
    private SpriteRenderer[,] previewDisplay = new SpriteRenderer[GridSizeX, GridSizeY];

    public void UpdateDisplayPreview()
    {
        if (!IsOpenTest) return;

        for (int i = 0; i < GridSizeX; i++)
        {
            for (int j = 0; j < GridSizeY; j++)
            {
                var active = Grid[i, j];
                var sprite = previewDisplay[i, j];

                sprite.color = active ? Color.green : Color.red;
            }
        }
    }

    #endregion
    
    private void Awake()
    {
        Instance = this;

        if (IsOpenTest)
        {
            for (int i = 0; i < GridSizeX; i++)
            {
                for (int j = 0; j < GridSizeY; j++)
                {
                    var sprite = Instantiate(displayDataPrefabs, transform);
                    sprite.transform.position = new Vector3(i, j, 0);
                    previewDisplay[i, j] = sprite;
                }
            }
        }
    }
    
    public bool IsInside(List<Vector2> listCoords)
    {
        foreach (var coordinate in listCoords)
        {
            int x = Mathf.RoundToInt(coordinate.x);
            int y = Mathf.RoundToInt(coordinate.y);
            
            if (x < 0 || x >= GridSizeX)
            {
                return false;
            }
            
            if (y < 0 || y >= GridSizeY)
            {
                return false;
            }

            if (Grid[x, y])
            {
                return false;
            }
        }
        
        return true;
    }

    private void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        var index = Random.Range(0, listPrefabs.Count);
        var blockController = listPrefabs[index];
        var newBlock = Instantiate(blockController);
        Current = newBlock;
        listHistory.Add(newBlock);
        
        UpdateDisplayPreview();
    }

    private bool IsRowFull(int index)
    {
        for (int i = 0; i < GridSizeX; i++)
        {
            if (!Grid[i, index])
            {
                return false;
            }
        }
        
        return true;
    }

    public void UpdateRemoveObjectController()
    {
        for (int i = 0; i < GridSizeY; i++)
        {
            var isFull = IsRowFull(i);
            if (isFull)
            {
                foreach (var myBlock in listHistory)
                {
                    var willDestroy = new List<Transform>();
                    foreach (var piece in myBlock.ListPiece)
                    {
                        int y = Mathf.RoundToInt(piece.position.y);
                        if (y == i)
                        {
                            willDestroy.Add(piece);
                        }
                        else if (y > i)
                        {
                            var position = piece.position;
                            position.y--;
                            piece.position = position;
                        }
                    }

                    foreach (var item in willDestroy)
                    {
                        myBlock.ListPiece.Remove(item);
                        Destroy(item.gameObject);
                    }
                }

                for (int j = 0; j < GridSizeX; j++)
                    Grid[j, i] = false;

                for (int j = i+1; j < GridSizeY; j++)
                for (int k = 0; k < GridSizeX; k++)
                {
                    Grid[k, j - 1] = Grid[k, j];
                }
                
                UpdateRemoveObjectController();
                return;
            }
        }
    }

    public bool IsAnyTopRowCellFilled()
    {
        int topRowIndex = Grid.GetLength(1) - 1;
        for (int x = 0; x < Grid.GetLength(0); x++)
        {
            if (Grid[x, topRowIndex])
            {
                return true;
            }
        }
        return false;
    }
    
    public void GameOver()
    {
        Debug.Log("Game Over!");
        Time.timeScale = 0;
    }
}

