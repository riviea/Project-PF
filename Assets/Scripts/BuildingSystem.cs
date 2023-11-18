using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator;

    [SerializeField]
    private Grid grid;

    private bool isActive = false;

    public void TurnBuildingSystem(bool turn)
    {
        isActive = turn;
    }

    private void Update()
    {
        if (isActive)
        {
            mouseIndicator.SetActive(true);

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePosition.z = 0f;

            Vector3Int gridPosition = grid.WorldToCell(mousePosition);

            mouseIndicator.transform.position = grid.CellToWorld(gridPosition);
        }
        else
        {
            mouseIndicator.SetActive(false);
        }    
    }
}
