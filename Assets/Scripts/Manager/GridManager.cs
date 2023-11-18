using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private Tilemap groundMap;
    [SerializeField]
    private Tilemap wallMap;

    [SerializeField]
    private GameObject character;

    [SerializeField]
    private Rigidbody2D characterRigidbody;

    List<Node> path;
    private bool isMoving = false;
    private int currentIndex = 0;

    private Vector3Int[,] cells;
    private BoundsInt bounds;

    private Astar astar;

    private void Start()
    {
        groundMap.CompressBounds();
        wallMap.CompressBounds();

        bounds = groundMap.cellBounds;

        CreateGrid();

        astar = new Astar();
    }
    private void Update()
    {
        if(Input.GetMouseButton(0) && !isMoving)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3 gridPosition = grid.CellToWorld(grid.WorldToCell(mousePosition));

            Vector2Int characterPos = new Vector2Int();
            characterPos.x = (int)character.transform.position.x;
            characterPos.y = (int)character.transform.position.y;

            Vector2Int targetPosition = new Vector2Int();
            targetPosition.x = (int)gridPosition.x;
            targetPosition.y = (int)gridPosition.y;

           path = astar.CreatePath(cells, characterPos, targetPosition);

            isMoving = true;
        }

        if (isMoving)
        {
            Vector2 characterPos = new Vector2();
            characterPos.x = character.transform.position.x;
            characterPos.y = character.transform.position.y;

            Vector2 targetPos = new Vector2();
            targetPos.x = path[currentIndex].x;
            targetPos.y = path[currentIndex].y;

            Vector2 dir = (characterPos - targetPos).normalized;

            if (characterPos == targetPos)
            {
                if (!(currentIndex == path.Count))
                    currentIndex++;
                
                if (currentIndex == path.Count)
                {
                    currentIndex = 0;
                    characterRigidbody.velocity = Vector2.zero;
                    isMoving = false;

                    return;
                }

                characterPos.x = targetPos.x;
                characterPos.y = targetPos.y;

                targetPos.x = path[currentIndex].x;
                targetPos.y = path[currentIndex].y;

                dir = (characterPos - targetPos);

                characterRigidbody.velocity = (dir * -1) * 10f;
            }
            else
                characterRigidbody.velocity = (dir * -1) * 10f;
            
        
        
        
        
        
        
        
        }
    }

 



    private void CreateGrid()
    {
        cells = new Vector3Int[bounds.size.x, bounds.size.y];

        for(int x = bounds.xMin, i = 0; i < (bounds.size.x); x++, i++)
        {
            for (int y = bounds.yMin, j = 0; j < (bounds.size.y); y++, j++)
            {
                
                if(wallMap.HasTile(new Vector3Int(x,y,0)))
                {
                    cells[i, j] = new Vector3Int(x, y, 1);
                }
                else if (groundMap.HasTile(new Vector3Int(x, y, 0)))
                {
                    cells[i, j] = new Vector3Int(x, y, 0);
                }
                else
                {
                    cells[i, j] = new Vector3Int(x, y, 1);
                }
            }
        }
    }
}
