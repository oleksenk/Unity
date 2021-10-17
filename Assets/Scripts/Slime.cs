using System;
using System.Collections;
using System.Collections.Generic;
using  UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UI;

public class Slime : MonoBehaviour
{
    [SerializeField] private float _attackRange;
    [SerializeField] private LayerMask _whatIsPlayer;
    [SerializeField] private Transform _Poz;
    [SerializeField] private Rigidbody2D _bullet;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private bool _faceRight;
    [SerializeField] private int _maxHp;
    [SerializeField] private Slider _slider;

    [Header("Animation")] [SerializeField] private Animator _animator;
    [SerializeField] private string _shootAnimationKey;
    private int _currentHp;
    private bool _canShoot;

    private int CurrentHp
    {
        get => _currentHp;
        set
        {
            _currentHp = value;
            _slider.value = value;
        }
    }
    
    private void Start()
    {
        _slider.maxValue = _maxHp;
        CurrentHp = _maxHp;
    }

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(_attackRange, 1, 0));
    }

    private void FixedUpdate()
    {
        if (_canShoot)
        {
            return;
        }

        CheckIfCanShoot();
    }

    private void CheckIfCanShoot()
    {
        Collider2D player = Physics2D.OverlapBox(transform.position, new Vector2(_attackRange, 1), 0, _whatIsPlayer);
        if (player != null)
        {
            _canShoot = true;
            StartShoot(player.transform.position);
        }
        else
        {
            _canShoot = false;
        }
    }
    
    private void StartShoot(Vector2 playerPosition)
    {
        if (transform.position.x > playerPosition.x && _faceRight || transform.position.x < playerPosition.x && !_faceRight)
        {
            _faceRight = !_faceRight;
            transform.Rotate(0,180,0);
        }
        _animator.SetBool(_shootAnimationKey, true);
    }

    public void Shoot()
    {
        Rigidbody2D bullet = Instantiate(_bullet);
        bullet.position = _Poz.position;
        bullet.velocity = _projectileSpeed * transform.right;
        _animator.SetBool(_shootAnimationKey, false);
        Invoke(nameof(CheckIfCanShoot),1f);
    }
    
    public void TakeDamage(int damage)
    {
        CurrentHp -= damage;
        if (CurrentHp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
 