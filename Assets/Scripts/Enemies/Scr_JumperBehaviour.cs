using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Scr_JumperBehaviour : MonoBehaviour
{
    [Header("Enemy Properties")]
    [SerializeField] private float health = 100f;
    [Range(2, 10)] [SerializeField] private float moveSpeed = 20f;
    [Range(6000, 15000)] [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float timeToJump = 2f;
    [SerializeField] private float timeToMoveAfterHit = 1f;

    [Header("Referentes")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private SpriteRenderer jumperSprite;
    [SerializeField] private ParticleSystem deathParticles;

    private bool attack;
    private bool dead;
    private bool active;
    private float moveTimer;
    private float jumpTimer;
    private Animator anim;
    private GameObject player;
    private Rigidbody2D rb;

    Scr_PlayerShooting playerShooting;

    private void Start()
    {
        playerShooting = GameObject.Find("New Player").GetComponent<Scr_PlayerShooting>();
        player = playerShooting.gameObject;
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        healthSlider.maxValue = health;
        healthSlider.value = health;
        healthSlider.gameObject.SetActive(false);
        moveTimer = timeToMoveAfterHit;
    }

    private void Update()
    {
        healthSlider.value = health;

        if (attack && !dead)
        {
            transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), player.transform.position, moveSpeed * Time.deltaTime);

            jumpTimer -= Time.deltaTime;

            if (jumpTimer <= 0)
            {
                rb.AddForce(Vector2.up * jumpForce);
                jumpTimer = timeToJump;
            }
        }

        else if (active)
        {
            moveTimer -= Time.deltaTime;

            if (moveTimer <= 0)
            {
                moveTimer = timeToMoveAfterHit;
                attack = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MG_proyectile")
            TakeDamage(playerShooting.MG_damage);

        else if (collision.gameObject.tag == "SG_proyectile")
            TakeDamage(playerShooting.SG_damage);

        else if (collision.gameObject.tag == "Player")
            attack = false;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.tag == "FT_proyectile")
            TakeDamage(playerShooting.FT_damage);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            active = true;
            attack = true;
        }
    }

    private void TakeDamage(float amount)
    {
        healthSlider.gameObject.SetActive(true);
        anim.SetTrigger("TakeDamage");
        health -= amount;

        if (health <= 0)
            DeathFX();
    }

    private void DeathFX()
    {
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GetComponent<CapsuleCollider2D>().enabled = false;
        healthSlider.gameObject.SetActive(false);
        jumperSprite.enabled = false;
        dead = true;
        deathParticles.Play();

        Invoke("Death", 2);
    }

    private void Death()
    {
        Destroy(gameObject);
    }
}