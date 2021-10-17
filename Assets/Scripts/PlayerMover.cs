using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    
    [SerializeField] private float _speed;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private float _jumpForce;
    [SerializeField] private Transform _groundChecker;
    [SerializeField] private Transform FirePosition;
    [SerializeField] private GameObject Fire;
    [SerializeField] private float _groundCheckerRadius;
    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private LayerMask _whatIsCell;
    [SerializeField] private Collider2D _headCollider;
    [SerializeField] private float _headCheckerRadius;
    [SerializeField] private Transform _headChecker;
    [SerializeField] private int _maxHp;
    [SerializeField] private int _maxMp;
    
    [Header(("Animation"))]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _runAnimatorKey;
    [SerializeField] private string _jumpAnimatorKey;
    [SerializeField] private string _crouchAnimatorKey;
    [SerializeField] private string _hurtAnimationKey;

    [Header("UI")] 
    [SerializeField] private TMP_Text _coinAmountText;
    [SerializeField] private Slider _hpBar;
    [SerializeField] private Slider _mpBar;
    private float _horizontalDirection;
    private float _verticalDirection;
    private bool _jump;
    private bool _crawl;
    private int _coinsAmount;
    private int _currentHp;
    private int _currentMp;

    private float _lastPushTime;
    public bool CanClimb { get;  set; }
    public int CoinsAmount
    {
        get => _coinsAmount;
        set
        {
            _coinsAmount = value;
            _coinAmountText.text = value.ToString();
        }
    }
    private int CurrentHp
    {
        get => _currentHp;
        set
        {
            if (value > _maxHp)
                value = _maxHp;
            _currentHp = value;
            _hpBar.value = value;
        }
    }
    private int CurrentMp
    {
        get => _currentMp;
        set
        {
            if (value > _maxMp)
                value = _maxMp;
            _currentMp = value;
            _mpBar.value = value;
        }
    }

    private void Start()
    {
        _hpBar.maxValue = _maxHp;
        _mpBar.maxValue = _maxMp;
        CoinsAmount = 0;
        CurrentHp = _maxHp;
        CurrentMp = _maxMp;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        //_rigidbody.AddForce(new Vector2(50_direction,0),ForceMode2D.Impulse);
        //_rigidbody.MovePosition(_rigidbody.position+new Vector2(_direction1,0));
        //_transform.position += new Vector3(_direction1,0,0);
        //_transform.Translate(new Vector3(1_direction,0,0));
        _horizontalDirection = Input.GetAxisRaw("Horizontal");
        _verticalDirection = Input.GetAxisRaw("Vertical");
        _animator.SetFloat(_runAnimatorKey, Mathf.Abs(_horizontalDirection));
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jump=true;
        }
        if (_horizontalDirection > 0 && _spriteRenderer.flipX)
        {
            _spriteRenderer.flipX = false;
        }
        else if(_horizontalDirection<0 && !_spriteRenderer.flipX)
        {
            _spriteRenderer.flipX = true;
        }

        _crawl = Input.GetKey(KeyCode.C);
        
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            Instantiate(Fire, FirePosition.position, transform.rotation);
        }
    }

    private void FixedUpdate()
    {
        bool canJump = Physics2D.OverlapCircle(_groundChecker.position , _groundCheckerRadius,_whatIsGround);
        if (_animator.GetBool(_hurtAnimationKey))
        {
            if (Time.time - _lastPushTime > 0.2f && canJump)
            {
                _animator.SetBool(_hurtAnimationKey, false);
            }
            return;
        }
        _rigidbody.velocity = new Vector2( _horizontalDirection * _speed , _rigidbody.velocity.y);

        if (CanClimb)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _verticalDirection * _speed);
            _rigidbody.gravityScale = 0;
        }
        else
        {
            _rigidbody.gravityScale = 3;
        }

        
        bool canStand = !Physics2D.OverlapCircle(_headChecker.position , _headCheckerRadius,_whatIsCell);
        
        _headCollider.enabled = !_crawl && canStand;
        if (_jump && canJump)
        {
            _rigidbody.AddForce(Vector2.up * _jumpForce);
            _jump = false;
        }

        _animator.SetBool(_jumpAnimatorKey, !canJump);
        _animator.SetBool(_crouchAnimatorKey, !_headCollider.enabled);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_groundChecker.position,_groundCheckerRadius);
        Gizmos.color=Color.green;
        Gizmos.DrawWireSphere(_headChecker.position,_headCheckerRadius);
    }

    public void AddHp(int hpPoints)
    {
        int missingHp = _maxHp - CurrentHp;
        int pointToAddHp = missingHp > hpPoints ? hpPoints : missingHp;
        StartCoroutine(RestoreHP(pointToAddHp));
    }
    private IEnumerator RestoreHP(int pointToAddHp)
    {
        
        while ( pointToAddHp!=0)
        {
            pointToAddHp--;
            CurrentHp++;
            yield return new WaitForSeconds(0.2f);
        }
    }
    public void TakeDamage(int damage, float pushPower = 0, float enemyPosX = 0)
    {
        if (_animator.GetBool(_hurtAnimationKey))
        {
            return;
        }
        
        CurrentHp -= damage;
        if (CurrentHp <= 0)
        {
            gameObject.SetActive(false);
            Invoke(nameof(ReloadScene),1f);
        }

        if (pushPower != 0)
        {
            _lastPushTime = Time.time;
            int direction = transform.position.x > enemyPosX ? 1 : -1;
            _rigidbody.AddForce(new Vector2(direction * pushPower/2, pushPower));
            _animator.SetBool(_hurtAnimationKey, true);
        }
    }
    public void AddMp(int mpPoints)
    {
        int missingMp = _maxMp - CurrentMp;
        int pointToAddMp = missingMp > mpPoints ? mpPoints : missingMp;
        StartCoroutine(RestoreMP(pointToAddMp));
    }
    private IEnumerator RestoreMP(int pointToAddMp)
    {
        
        while ( pointToAddMp!=0)
        {
            pointToAddMp--;
            CurrentMp++;
            yield return new WaitForSeconds(0.2f);
        }
    }
    
    public void AddCoints(int CointsPoints)
    {
        Debug.Log(message:"Coints " + CointsPoints);
    }
    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}