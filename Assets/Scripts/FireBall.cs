using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] private PlayerMover _player;
    
    private float _FireSpeed = 7f;
    
    private void Update()
    {
        transform.Translate(Vector2.right * _FireSpeed * Time.deltaTime);
        print(_player == null);
        Invoke(nameof(DestroyFire),2f);
        
    }
    
    private void DestroyFire()
    {
        Destroy(gameObject);
    }
    
}