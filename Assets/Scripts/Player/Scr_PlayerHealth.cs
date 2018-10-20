using System.Collections;
using System.Collections.Generic;
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

    private bool canTakeDamage = true;
    private float damageTimer;
    private Animator anim;

    private void Start()
    {
        ResetDamageTimer();

        anim = GetComponent<Animator>();

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
            Death();
        else
            canTakeDamage = false;
    }

    private void ResetDamageTimer()
    {
        damageTimer = invulnerableTime;
    }

    public void Death()
    {
        print("Playerdeath");
    }
}