using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] private int _CointsPoints;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMover player = other.GetComponent<PlayerMover>();
        if (player != null)
        {
            Debug.Log("CoinsAdded");
            player.CoinsAmount += _CointsPoints;
            Destroy(gameObject);
        }
    }
}