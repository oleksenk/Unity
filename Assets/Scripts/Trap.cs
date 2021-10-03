using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _damageDelay ;
    private float _lastDamageTime;
    private PlayerMover _player;
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMover player = other.GetComponent<PlayerMover>();
        if (player != null)
        {
            _player = player;
        }
       
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerMover player = other.GetComponent<PlayerMover>();
        if (player == _player)
        {
            _player = null;
        }
    }

    public void Update()
    {
        if (_player != null && Time.time -_lastDamageTime>_damageDelay)
        {
            _lastDamageTime = Time.time;
            _player.TakeDamage(_damage);
        }
    }
}
