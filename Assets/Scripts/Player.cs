using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private void Awake()
    {
        this.name = ClientSystem.clientSystem.playerName;
    }
}
