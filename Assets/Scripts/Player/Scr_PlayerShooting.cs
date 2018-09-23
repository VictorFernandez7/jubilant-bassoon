using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Scr_PlayerShooting : MonoBehaviour
{
    [SerializeField] GameObject gun;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] public Transform gunEnd;
    [SerializeField] float shootingForce = 20f;
    [SerializeField] int ammo = 10;
    [SerializeField] int reloadSpeed = 2;
    [SerializeField] Slider reloadSlider;
    [SerializeField] Slider ammoSlider;

    [HideInInspector] public bool gunLookingUp = false;
    [HideInInspector] public bool gunLookingDown = false;

    bool reloading = false;
    int currentAmmo;

    //Vector3 direction;
    //float angle;

    private void Start()
    {
        currentAmmo = ammo;
        reloadSlider.maxValue = ammo;
    }

    private void Update()
    {
        /*direction = Input.mousePosition - gun.transform.position;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gun.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);*/

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
        
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        ammoSlider.value = currentAmmo;

        if (reloading)
        {
            reloadSlider.value += reloadSpeed * Time.deltaTime;

            Debug.Log("Start Reload");

            if (reloadSlider.value == reloadSlider.maxValue)
            {
                Debug.Log("End Reload");

                reloading = false;
                reloadSlider.gameObject.SetActive(false);
                currentAmmo = ammo;
                reloadSlider.value = 0;
            }
        }        
    }

    void Shoot()
    {
        currentAmmo -= 1;

        if (currentAmmo > 0)
        {
            Instantiate(bulletPrefab, gunEnd.position, gunEnd.rotation);

            if (gunLookingDown)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

                GetComponent<Rigidbody2D>().AddForce(Vector2.up * shootingForce);
            }
        }

        else
            Reload();
    }

    void Reload()
    {
        reloadSlider.gameObject.SetActive(true);
        reloading = true;
    }
}