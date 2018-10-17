using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Scr_JumperBehaviour : MonoBehaviour
{
    [SerializeField] float health = 100;
    [SerializeField] Slider healthSlider;

    Scr_PlayerShooting playerShooting;

    private void Start()
    {
        playerShooting = GameObject.Find("New Player").GetComponent<Scr_PlayerShooting>();

        healthSlider.maxValue = health;
        healthSlider.value = health;
        healthSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        healthSlider.value = health;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MG_proyectile")
            TakeDamage(playerShooting.MG_damage);

        else if (collision.gameObject.tag == "SG_proyectile")
            TakeDamage(playerShooting.SG_damage);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.tag == "FT_proyectile")
            TakeDamage(playerShooting.FT_damage);
    }

    public void TakeDamage(float amount)
    {
        healthSlider.gameObject.SetActive(true);
        health -= amount;

        if (health <= 0)
            Death();
    }

    void Death()
    {
        Destroy(gameObject);
    }
}