using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private void Awake()
    {
        instance = GetComponent<UIManager>();
        DontDestroyOnLoad(instance);
    }

    public void SwitchActive(GameObject gameObject)
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void LoadScene(int id)
    {
        SceneManager.LoadScene(id);
    }
}
