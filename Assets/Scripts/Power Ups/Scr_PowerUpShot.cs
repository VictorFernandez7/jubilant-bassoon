using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_PowerUpShot : MonoBehaviour
{
    public Scr_PlayerShooting player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.powerShotActive = true;
        }
    }
}