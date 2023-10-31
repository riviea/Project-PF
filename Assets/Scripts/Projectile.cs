using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rigidbody2;

    [SerializeField] private float selfDestroyTime;
    private float timer;

    [SerializeField] private float speed;

    private void Awake()
    {
        rigidbody2 = GetComponent<Rigidbody2D>();        
    }

    private void Start()
    {
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > selfDestroyTime)
            Destroy(gameObject);
    }

    public void Launch(Vector2 firePos, Vector2 mousePos)
    {
        //firePos로 Projectile 위치를 초기화
        transform.localPosition = firePos;
        
        //마우스 위치와 발사 위치를 통해 방향을 얻는다
        Vector2 direction = (mousePos - firePos).normalized;

        //얻은 방향으로 발사
        rigidbody2.AddForce(direction * speed);
    }
}
