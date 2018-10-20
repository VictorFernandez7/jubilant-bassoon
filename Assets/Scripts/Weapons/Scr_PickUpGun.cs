using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_PickUpGun : MonoBehaviour
{
    [SerializeField] int gunType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Scr_PlayerShooting>().ChangeWeapon(gunType);
            Destroy(gameObject);
        }
    }
}