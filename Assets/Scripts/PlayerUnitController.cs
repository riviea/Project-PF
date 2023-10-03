using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUnitController : UnitController
{
    void OnMove(InputValue input)
    {
        Vector2 vec = input.Get<Vector2>();
        CallMoveEvent(vec);
    }
}
