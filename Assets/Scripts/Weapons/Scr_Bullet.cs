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

    [Header("Particle Systems")]
    [SerializeField] ParticleSystem trailParticle;
    [SerializeField] ParticleSystem collisionParticle;

    Rigidbody2D rb;
    bool collisionWIthTarget = false;

    public enum BulletType
    {
        normal,
        powerShot
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (!collisionWIthTarget)
            rb.velocity = transform.right * speed;

        Invoke("ActivateCollider", 0.5f);
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
        {
            collisionWIthTarget = true;
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            GetComponent<CircleCollider2D>().enabled = false;
            trailParticle.Stop();
            collisionParticle.Play();

            Invoke("DestroyBullet", 1);
        }
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }

    void ActivateCollider()
    {
        GetComponent<CircleCollider2D>().enabled = true;
    }
}