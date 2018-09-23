using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_PlayerShooting : MonoBehaviour
{
    [SerializeField] GameObject gun;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] public Transform gunEnd;
    [SerializeField] float shootingForce = 20f;

    [HideInInspector] public bool gunLookingUp = false;
    [HideInInspector] public bool gunLookingDown = false;

    //Vector3 direction;
    //float angle;

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
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, gunEnd.position, gunEnd.rotation);

        if (gunLookingDown)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

            GetComponent<Rigidbody2D>().AddForce(Vector2.up * shootingForce);
        }
    }
}