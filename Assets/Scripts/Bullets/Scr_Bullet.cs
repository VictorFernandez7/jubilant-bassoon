using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Bullet : MonoBehaviour
{
    [Header("Type selection")]
    [SerializeField] BulletType bulletType;

    [Header("Bullet Parameters")]
    public float speed = 20f;
    public float timeToDestroy = 3;

    Rigidbody2D rb;

    public enum BulletType
    {
        normal,
        powerShot
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.velocity = transform.right * speed;
    }

    private void Update()
    {
        timeToDestroy -= Time.deltaTime;

        if (timeToDestroy <= 0)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player")
            Destroy(gameObject);
    }
}