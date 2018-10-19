using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Scr_OctopusBehaviour : MonoBehaviour
{
    [Header("Enemy Properties")]
    [SerializeField] private float health = 100f;
    [Range(1, 10)] [SerializeField] private float moveSpeed = 20f;
    [Range(0.5f, 2)] [SerializeField] private float timeToChangePos = 1f;
    [Range(0.5f, 2)] [SerializeField] private float timeToShoot = 1.5f;
    [SerializeField] private float pauseTime = 1f;

    [Header("Referentes")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private SpriteRenderer octopusSprite;
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private GameObject enemyBullet;

    private bool dead;
    private bool move;
    private float moveTimer;
    private float shootTimer;
    private Vector3 targetDirection;
    private GameObject player;
    private Rigidbody2D rb;
    private Animator anim;

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

        MoveTimerReset();
        ShootTimerReset();
        GiveNewPosition();

        move = true;
    }

    private void Update()
    {
        healthSlider.value = health;

        if (move)
        {
            moveTimer -= Time.deltaTime;
            shootTimer -= Time.deltaTime;

            transform.Translate(targetDirection.x * Time.deltaTime, targetDirection.y * Time.deltaTime, 0);

            if (moveTimer <= 0)
            {
                GiveNewPosition();
            }

            if (shootTimer <= 0)
            {
                Shoot();
            }
        }

        if (targetDirection.x < 0)
        {
            if (octopusSprite.flipX)
                octopusSprite.flipX = false;
        }

        else if (targetDirection.x > 0)
        {
            if (!octopusSprite.flipX)
                octopusSprite.flipX = true;
        }

        //print(targetDirection);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MG_proyectile")
            TakeDamage(playerShooting.MG_damage);

        else if (collision.gameObject.tag == "SG_proyectile")
            TakeDamage(playerShooting.SG_damage);

        else if (collision.gameObject.tag == "Player")
            move = false;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.tag == "FT_proyectile")
            TakeDamage(playerShooting.FT_damage);
    }

    void Shoot()
    {
        Instantiate(enemyBullet, transform.position,transform.rotation);
        ShootTimerReset();
    }

    private void GiveNewPosition()
    {
        targetDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);

        MoveTimerReset();
    }

    private void MoveTimerReset()
    {
        moveTimer = timeToChangePos;
    }

    private void ShootTimerReset()
    {
        shootTimer = timeToShoot;
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
        octopusSprite.enabled = false;
        dead = true;
        deathParticles.Play();

        Invoke("Death", 2);
    }

    private void Death()
    {
        Destroy(gameObject);
    }
}