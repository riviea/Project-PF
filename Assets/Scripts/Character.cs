using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private Transform characterTransform;
    [SerializeField] private float character_HP;

    private void Awake()
    {
        characterTransform = GetComponent<Transform>();
    }

    public string GetCharacterInfo()
    {
        string info = this.name + "/" + characterTransform.localPosition.x.ToString() + "/" + characterTransform.localPosition.y.ToString();
        return info;
    }
}
