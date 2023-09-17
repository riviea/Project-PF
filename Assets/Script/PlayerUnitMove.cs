using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerUnitMove : MonoBehaviour
{
    PlayerUnitController _controller;
    Vector2 _direction = Vector2.zero;
    Rigidbody2D _rigidbody;
    [SerializeField] private float _unitSpeed = 5f;

    private void Awake()
    {
        _controller = GetComponent<PlayerUnitController>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _controller.OnMoveEvent += Move;
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = _direction * _unitSpeed;
    }

    void Move(Vector2 input)
    {
        _direction = input;
    }
}
