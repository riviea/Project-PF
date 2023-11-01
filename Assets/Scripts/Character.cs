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
        /*
        "Prefabs/Player" �κ��� �ӽ÷� �� ������ ���߿� �����ؾ��Ѵ�(11/01)
        1. ��� ĳ���� Player Prefab�� ���� �ƴϹǷ� ���߿� ����

        */

        string info = ((int)ClientSystem.GameObjectType.PLAYER).ToString() + "~" +
            "Prefabs/Player" + "~" +
            this.name + "~" +
            characterTransform.localPosition.x.ToString() + "~" +
            characterTransform.localPosition.y.ToString();
        return info;
    }
}
