using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    [SerializeField] private float _walkRange;
    [SerializeField] private bool _face_Right;
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private int _damage;
    [SerializeField] private float _pushPower;
    private Vector2 _startPosition;
    private int _direction=1;
    private float _lastAttackTime;
    private Vector2 _drawPosition
    {
        get
        {
            if (_startPosition == Vector2.zero)
            {
                return transform.position;
            }
            else
            {
                return _startPosition;
            }
        }
    }

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_drawPosition, new Vector3(1,_walkRange*2,0));
    }

    private void Update()
    {
        if (_face_Right && transform.position.y > _startPosition.y + _walkRange)
        {
            Flip();
        }
        else if (!_face_Right && transform.position.y < _startPosition.y - _walkRange)
        {
            Flip();
        }
    }

    private void Flip()
    {
        _face_Right = !_face_Right;
        transform.Rotate(180,0,0);
        _direction *= -1;
    }

    private void FixedUpdate()
    {
        _rigidbody2D.velocity=Vector2.up * _direction * _speed;

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        PlayerMover player = other.collider.GetComponent<PlayerMover>();
        if (player != null && Time.time - _lastAttackTime > 0.2f)
        {
            _lastAttackTime = Time.time;
            player.TakeDamage(_damage,_pushPower,transform.position.x);
        }
    }
}