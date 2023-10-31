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
        //firePos�� Projectile ��ġ�� �ʱ�ȭ
        transform.localPosition = firePos;
        
        //���콺 ��ġ�� �߻� ��ġ�� ���� ������ ��´�
        Vector2 direction = (mousePos - firePos).normalized;

        //���� �������� �߻�
        rigidbody2.AddForce(direction * speed);
    }
}
