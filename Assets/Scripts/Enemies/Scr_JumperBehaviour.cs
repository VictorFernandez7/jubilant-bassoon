using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Scr_JumperBehaviour : MonoBehaviour
{
    [Header("Enemy Properties")]
    [SerializeField] float health = 100f;
    [SerializeField] float moveSpeed = 20f;
    [SerializeField] float jumpForce = 20f;
    [SerializeField] float timeToJump = 2f;
    [SerializeField] float timeToMoveAfterHit = 1f;
    [SerializeField] ParticleSystem deathParticles;

    [Header("Referentes")]
    [SerializeField] Slider healthSlider;
    [SerializeField] SpriteRenderer jumperSprite;

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

    public void TakeDamage(float amount)
    {
        healthSlider.gameObject.SetActive(true);
        anim.SetTrigger("TakeDamage");
        health -= amount;

        if (health <= 0)
            DeathFX();
    }

    void DeathFX()
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

    void Death()
    {
        Destroy(gameObject);
    }
}