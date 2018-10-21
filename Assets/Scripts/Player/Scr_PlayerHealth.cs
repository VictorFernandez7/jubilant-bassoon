using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Scr_PlayerController))]
[RequireComponent(typeof(Scr_PlayerInput))]
[RequireComponent(typeof(Scr_PlayerShooting))]
public class Scr_PlayerHealth : MonoBehaviour
{
    [Header("Health Properties")]
    [SerializeField] float health = 3;
    [SerializeField] float invulnerableTime = 2;

    [Header("Damage Properties")]
    [Range(0.5f, 1)] [SerializeField] float proyectileDamage = 0.5f;
    [Range(0.5f, 1)] [SerializeField] float enemyCollisionDamage = 0.5f;

    [Header("References")]
    [SerializeField] Slider healthSlider;
    [SerializeField] ParticleSystem deathParticles;

    private bool canTakeDamage = true;
    private float damageTimer;
    private Animator anim;

    Scr_PlayerInput playerInput;

    private void Start()
    {
        ResetDamageTimer();

        anim = GetComponent<Animator>();
        playerInput = GameObject.Find("New Player").GetComponent<Scr_PlayerInput>();

        healthSlider.maxValue = health;
    }

    private void Update()
    {
        anim.SetBool("CanTakeDamage", canTakeDamage);

        healthSlider.value = health;

        if (!canTakeDamage)
        {
            damageTimer -= Time.deltaTime;

            if (damageTimer <= 0)
                canTakeDamage = true;
        }

        else if (damageTimer != invulnerableTime)
            ResetDamageTimer();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (canTakeDamage)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                TakeDamage(enemyCollisionDamage);
            }

            else if (collision.gameObject.tag == "EnemyBullet")
            {
                TakeDamage(proyectileDamage);
            }
        }
    }

    private void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0)
            DeathFX();
        else
            canTakeDamage = false;
    }

    private void ResetDamageTimer()
    {
        damageTimer = invulnerableTime;
    }

    private void DeathFX()
    {
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        playerInput.dead = true;
        deathParticles.Play();

        Invoke("Death", 2);
    }

    private void Death()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}