using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private int _damage;
    [SerializeField] private Rigidbody2D _rigidbody;

    public void StartFly(Vector2 direction)
    {
        _rigidbody.velocity = direction * _speed;
        Invoke(nameof(Destroy), 2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMover player = other.GetComponent<PlayerMover>();
        if (player != null)
        {
            player.TakeDamage(_damage);
        }
        Destroy();
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
