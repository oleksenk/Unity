using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] private int _coinsToNextLevel;
    [SerializeField] private int _levelToLaod;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _openDoorsSprite;
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMover player = other.GetComponent<PlayerMover>();
        if (player != null && player.CoinsAmount >= _coinsToNextLevel)
        {
            Debug.Log("Doors open");
            _spriteRenderer.sprite = _openDoorsSprite;
            Invoke(nameof(LoadNextScene), 1f);
        }
        
    }
    
    private void LoadNextScene()
    {
        SceneManager.LoadScene(_levelToLaod);
    }
}
