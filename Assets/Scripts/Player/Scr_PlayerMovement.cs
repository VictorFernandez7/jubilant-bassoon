using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_PlayerMovement : MonoBehaviour
{
    public Scr_PlayerController controller;

    public float runSpeed = 40f;

    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (horizontalMove != 0)
            controller.anim.SetBool("Moving", true);
        else
            controller.anim.SetBool("Moving", false);

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;

            controller.anim.SetBool("Jumping", true);
        }

        else if (Input.GetButtonUp("Jump"))
        {
            jump = false;

            controller.anim.SetBool("Jumping", false);
        }

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;

            controller.anim.SetBool("Crouching", true);
        }

        else if(Input.GetButtonUp("Crouch"))
        {
            crouch = false;

            controller.anim.SetBool("Crouching", false);
        }
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }
}