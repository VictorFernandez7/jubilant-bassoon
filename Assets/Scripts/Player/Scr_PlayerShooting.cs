using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Scr_PlayerShooting : MonoBehaviour
{
    [SerializeField] GameObject gun;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject powerBulletPrefab;
    [SerializeField] public Transform gunEnd;
    [SerializeField] float shootingForce = 20f;
    [SerializeField] int ammo = 10;
    [SerializeField] int reloadSpeed = 2;
    [SerializeField] Slider reloadSlider;
    [SerializeField] Slider ammoSlider;

    [HideInInspector] public bool gunLookingUp = false;
    [HideInInspector] public bool gunLookingDown = false;
    [HideInInspector] public bool powerShotActive = false;

    bool reloading = false;
    int currentAmmo;

    private void Start()
    {
        currentAmmo = ammo;
        reloadSlider.maxValue = ammo;
    }

    private void Update()
    {
        ammoSlider.value = currentAmmo;

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot(powerShotActive);
        }

        if (Input.GetAxisRaw("Vertical") > 0.5f && gunLookingUp == false)
        {
            gun.transform.Rotate(0f, 0f, 90f);

            gunLookingUp = true;
        }

        else if (Input.GetAxisRaw("Vertical") < 0.5f && gunLookingUp == true)
        {
            gun.transform.Rotate(0f, 0f, -90f);

            gunLookingUp = false;
        }

        if (Input.GetAxisRaw("Vertical") < -0.5f && gunLookingDown == false && !GetComponent<Scr_PlayerController>().m_Grounded)
        {
            gun.transform.Rotate(0f, 0f, -90f);

            gunLookingDown = true;
        }

        else if ((Input.GetAxisRaw("Vertical") > -0.5f && gunLookingDown == true) || (gunLookingDown == true && GetComponent<Scr_PlayerController>().m_Grounded))
        {
            gun.transform.Rotate(0f, 0f, 90f);

            gunLookingDown = false;
        }

        if (reloading)
        {
            reloadSlider.value += reloadSpeed * Time.deltaTime;

            if (reloadSlider.value == reloadSlider.maxValue)
            {
                reloading = false;
                reloadSlider.gameObject.SetActive(false);
                currentAmmo = ammo;
                reloadSlider.value = 0;
            }
        }        
    }

    public void Shoot(bool powerShot)
    {
        if (!powerShot)
        {
            currentAmmo -= 1;

            if (currentAmmo > 0)
            {
                Instantiate(bulletPrefab, gunEnd.position, gunEnd.rotation);

                if (gunLookingDown)
                {
                    GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);

                    GetComponent<Rigidbody2D>().AddForce(Vector2.up * shootingForce);
                }
            }

            else
                Reload(false);
        }

        else
        {
            currentAmmo -= 2;

            if (currentAmmo > 0)
            {
                Instantiate(powerBulletPrefab, gunEnd.position, gunEnd.rotation);

                if (gunLookingDown)
                {
                    GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);

                    GetComponent<Rigidbody2D>().AddForce(Vector2.up * shootingForce * 5);
                }
            }

            else
            {
                Reload(false);

                powerShotActive = false;
            }
        }        
    }

    public void Reload(bool powerUp)
    {
        if (!powerUp)
        {
            reloadSlider.gameObject.SetActive(true);
            reloading = true;
        }

        else
        {
            reloading = true;
            reloadSlider.value = reloadSlider.maxValue;
        }
    }
}