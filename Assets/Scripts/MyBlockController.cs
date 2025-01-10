using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBlockController : MonoBehaviour
{
    public List<Transform> ListPiece => listPiece;
    [SerializeField] private List<Transform> listPiece = new List<Transform>();

    private float delay;
    
    private void Start()
    {
        StartCoroutine(MoveDown());
    }

    IEnumerator MoveDown()
    {
        while (true)
        {
            delay = MyPlayerInput.Instance.fallSpeed;
            
            yield return new WaitForSeconds(delay);
            
            var isMovable = GameManager.Instance.IsInside(GetPreviewPosition());
            if (isMovable)
            {
                Move();    
            }
            else
            {
                foreach (var piece in listPiece)
                {
                    int x = Mathf.RoundToInt(piece.position.x);
                    int y = Mathf.RoundToInt(piece.position.y);

                    if (y >= GameManager.Instance.Grid.GetLength(1))
                    {
                        GameManager.Instance.GameOver();
                        yield break;
                    }
                    
                    GameManager.Instance.Grid[x, y] = true;
                }

                if (GameManager.Instance.IsAnyTopRowCellFilled())
                {
                    GameManager.Instance.GameOver();
                    yield break;
                }
                
                GameManager.Instance.UpdateRemoveObjectController();
                GameManager.Instance.Spawn();
                break;
            }
            
            Move();
        }
    }

    private List<Vector2> GetPreviewPosition()
    {
        var result = new List<Vector2>();
        foreach (var piece in listPiece)
        {
            var position = piece.position;
            position.y--;
            result.Add(position);
        }

        return result;
    }
    
    private void Move()
    {
        var position = transform.position;
        position.y -= 0.5f;
        transform.position = position;
    }
}