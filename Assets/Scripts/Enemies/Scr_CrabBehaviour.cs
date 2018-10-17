using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Scr_CrabBehaviour : MonoBehaviour
{
    [Header("Enemy Properties")]
    [SerializeField] float health = 100f;
    [SerializeField] float moveSpeed = 20f;
    [SerializeField] float timeToMoveAfterHit = 1f;
    [SerializeField] ParticleSystem deathParticles;

    [Header("Referentes")]
    [SerializeField] Slider healthSlider;
    [SerializeField] SpriteRenderer crabSprite;

    private int direction;
    private bool move;
    private bool dead;
    private float moveTimer;
    private float speedX;
    private Animator anim;

    Scr_PlayerShooting playerShooting;

    private void Start()
    {
        playerShooting = GameObject.Find("New Player").GetComponent<Scr_PlayerShooting>();
        anim = GetComponentInChildren<Animator>();

        healthSlider.maxValue = health;
        healthSlider.value = health;
        healthSlider.gameObject.SetActive(false);
        direction = Random.Range(0, 2);
        moveTimer = timeToMoveAfterHit;
    }

    private void Update()
    {
        healthSlider.value = health;
        anim.SetFloat("SpeedX", speedX);

        if (move && !dead)
        {
            if (direction == 0)
            {
                transform.Translate(-moveSpeed * Time.deltaTime, 0, 0);
                speedX = -1;

                if (crabSprite.flipX)
                    crabSprite.flipX = false;
            }

            else
            {
                transform.Translate(moveSpeed * Time.deltaTime, 0, 0);
                speedX = 1;

                if (!crabSprite.flipX)
                    crabSprite.flipX = true;
            }
        }

        else
        {
            moveTimer -= Time.deltaTime;
            speedX = 0;

            if (moveTimer <= 0)
            {
                moveTimer = timeToMoveAfterHit;
                move = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MG_proyectile")
            TakeDamage(playerShooting.MG_damage);

        else if (collision.gameObject.tag == "SG_proyectile")
            TakeDamage(playerShooting.SG_damage);

        else if (collision.gameObject.tag == "Obstacle")
            move = true;

        else if (collision.gameObject.tag == "Player")
            move = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            if (direction == 0)
                direction = 1;
            else
                direction = 0;
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.tag == "FT_proyectile")
            TakeDamage(playerShooting.FT_damage);
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
        crabSprite.enabled = false;
        dead = true;
        deathParticles.Play();

        Invoke("Death", 2);
    }

    void Death()
    {
        Destroy(gameObject);
    }
}