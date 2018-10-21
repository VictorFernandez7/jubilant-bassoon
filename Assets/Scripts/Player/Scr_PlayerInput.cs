using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Scr_PlayerController))]
[RequireComponent(typeof(Scr_PlayerHealth))]
[RequireComponent(typeof(Scr_PlayerShooting))]
public class Scr_PlayerInput : MonoBehaviour
{
    [HideInInspector] public bool dead;

    private bool crouch;
    private float horizontalMove;
    private Animator anim;
    private Scr_PlayerController playerController;
    private Scr_PlayerShooting playerShooting;

    private void Start()
    {
        playerController = GetComponent<Scr_PlayerController>();
        playerShooting = GetComponent<Scr_PlayerShooting>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!dead)
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * playerController.runSpeed;

            anim.SetBool("Shoot", playerShooting.shooting);

            if (Input.GetButtonDown("Crouch"))
                crouch = true;

            else if (Input.GetButtonUp("Crouch"))
                crouch = false;

            if (Input.GetButton("Fire1") && playerShooting.timer >= playerShooting.currentGuns[playerShooting.equipedGun].ShootRate)
            {
                if (!playerShooting.reloading)
                    playerShooting.Shoot(playerShooting.powerShotActive);
            }

            if (Input.GetButton("Fire2") && playerShooting.reloading)
            {
                playerShooting.reloadSlider.gameObject.SetActive(false);
                playerShooting.reloadSlider.value = 0;
                playerShooting.reloading = false;
            }

            if (Input.GetButtonDown("Fire1") && playerShooting.currentAmmo > 0)
                playerShooting.shooting = true;

            if (Input.GetButtonUp("Fire1"))
                playerShooting.shooting = false;

            if (Input.GetButtonDown("Reload") && playerShooting.currentAmmo != playerShooting.currentGuns[playerShooting.equipedGun].Ammo)
                playerShooting.Reload(false);
        }
    }

    private void FixedUpdate()
    {
        playerController.Move(horizontalMove * Time.fixedDeltaTime, crouch);
    }
}