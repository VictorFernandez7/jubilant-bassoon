using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Bullet : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;

    public Scr_PlayerShooting player;

    private void Start()
    {
        if (player.gunLookingUp || player.gunLookingDown)
            rb.velocity = transform.up * speed;
        else
            rb.velocity = transform.right * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player")
            Destroy(gameObject);
    }
}