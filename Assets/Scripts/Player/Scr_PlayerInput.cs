using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_PlayerInput : MonoBehaviour
{
    Scr_PlayerController playerController;
    Scr_PlayerShooting playerShooting;

    Animator anim;
    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;

    private void Start()
    {
        playerController = GetComponent<Scr_PlayerController>();
        playerShooting = GetComponent<Scr_PlayerShooting>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * playerController.runSpeed;

        anim.SetBool("Shoot", playerShooting.shooting);

        if (Input.GetButtonDown("Crouch"))
            crouch = true;

        else if(Input.GetButtonUp("Crouch"))
            crouch = false;

        if (Input.GetButton("Fire1") && playerShooting.timer >= playerShooting.ShootRate)
        {
            if (!playerShooting.reloading)
                playerShooting.Shoot(playerShooting.powerShotActive);

            else
            {
                playerShooting.reloadSlider.gameObject.SetActive(false);
                playerShooting.reloadSlider.value = 0;
                playerShooting.reloading = false;
            }
        }

        if (Input.GetButtonDown("Fire1"))
            playerShooting.shooting = true;

        if (Input.GetButtonUp("Fire1"))
            playerShooting.shooting = false;

        if (Input.GetButtonDown("Reload") && playerShooting.currentAmmo != playerShooting.ammo)
            playerShooting.Reload(false);
    }

    void FixedUpdate()
    {
        playerController.Move(horizontalMove * Time.fixedDeltaTime, crouch);
        jump = false;
    }
}