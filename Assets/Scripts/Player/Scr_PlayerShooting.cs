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
    [SerializeField] bool lateralImpulse = false;
    [SerializeField] Camera mainCamera;
    [SerializeField] float ShootRate;

    [HideInInspector] public bool gunLookingUp = false;
    [HideInInspector] public bool gunLookingDown = false;
    [HideInInspector] public bool powerShotActive = false;

    bool reloading = false;
    int currentAmmo;
    bool m_FacingRight = true;
    bool canShoot;

    Vector2 mousePos;
    Vector2 direction;

    private void Start()
    {
        currentAmmo = ammo;
        ammoSlider.maxValue = ammo;
    }

    private void Update()
    {
        ammoSlider.value = currentAmmo;

        mousePos = Input.mousePosition;
        mousePos = mainCamera.ScreenToWorldPoint(mousePos);
        direction = new Vector2(mousePos.x - gun.transform.position.x, mousePos.y - gun.transform.position.y);

        gun.transform.right = direction;

        if (Input.GetButton("Fire1"))
        {
            Shoot(powerShotActive);
        }

        if (Input.GetButtonDown("Reload"))
        {
            Reload(false);
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

                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);

                GetComponent<Rigidbody2D>().AddForce(- direction.normalized * shootingForce);
            }

            else
                Reload(false);
        }

        else
        {
            currentAmmo -= 1;

            if (currentAmmo > 0)
            {
                Instantiate(bulletPrefab, gunEnd.position, gunEnd.rotation);

                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);

                GetComponent<Rigidbody2D>().AddForce(-direction.normalized * shootingForce);
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