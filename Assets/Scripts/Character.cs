using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected float character_HP;
    [SerializeField] protected string prefab_Addr;

    public string GetCharacterInfo()
    {
        string info = ((int)ClientSystem.GameObjectType.PLAYER).ToString() + "~" +
            prefab_Addr + "~" +
            this.name + "~" +
            transform.localPosition.x.ToString() + "~" +
            transform.localPosition.y.ToString();

        return info;
    }
}
