using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_PlayerInput : MonoBehaviour
{
    Scr_PlayerController playerController;
    Scr_PlayerShooting playerShooting;

    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;

    private void Start()
    {
        playerController = GetComponent<Scr_PlayerController>();
        playerShooting = GetComponent<Scr_PlayerShooting>();
    }

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * playerController.runSpeed;

        if (Input.GetButtonDown("Crouch"))
            crouch = true;

        else if(Input.GetButtonUp("Crouch"))
            crouch = false;

        if (Input.GetButton("Fire1") && playerShooting.timer >= playerShooting.ShootRate)
        {
            playerShooting.Shoot(playerShooting.powerShotActive);
            playerShooting.shooting = true;
        }

        else
            playerShooting.shooting = false;


        if (Input.GetButtonDown("Reload"))
            playerShooting.Reload(false);
    }

    void FixedUpdate()
    {
        playerController.Move(horizontalMove * Time.fixedDeltaTime, crouch);
        jump = false;
    }
}