using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUnitController : UnitController
{
    void OnMove(InputValue input)
    {
        Vector2 vec = input.Get<Vector2>();

        if (vec == Vector2.left)
            Debug.Log("left");
        if (vec == Vector2.right)
            Debug.Log("right");
        if (vec == Vector2.up)
            Debug.Log("up");
        if (vec == Vector2.down)
            Debug.Log("down");

        CallMoveEvent(vec);
    }
}
