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
        "Prefabs/Player" 부분은 임시로 한 것으로 나중에 수정해야한다(11/01)
        1. 모든 캐릭터 Player Prefab인 것은 아니므로 나중에 수정

        */

        string info = ((int)ClientSystem.GameObjectType.PLAYER).ToString() + "~" +
            "Prefabs/Player" + "~" +
            this.name + "~" +
            characterTransform.localPosition.x.ToString() + "~" +
            characterTransform.localPosition.y.ToString();
        return info;
    }
}
