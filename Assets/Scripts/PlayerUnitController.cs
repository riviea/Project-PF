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
        CallMoveEvent(vec);
    }

    void OnFire()
    {
        Vector2 firePos = GameObject.Find("FirePoint").transform.position;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Debug.Log(firePos + " / " + mousePos);

        GameObject bullet = Resources.Load("Prefabs/Bullet") as GameObject;
        bullet = MonoBehaviour.Instantiate(bullet);

        Projectile bulletProjectile = bullet.GetComponent<Projectile>();
        bulletProjectile.Launch(firePos, mousePos);
    }
}
