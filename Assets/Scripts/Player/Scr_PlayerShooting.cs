using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Scr_PlayerShooting : MonoBehaviour
{
    [Header("Input selection")]
    [SerializeField] ControlMode controlMode;

    [Header("Shooting Properties")]
    [SerializeField] int ammo = 10;
    [Range(0, 1)] [SerializeField] public float ShootRate = 1;
    [SerializeField] int reloadSpeed = 2;
    [SerializeField] float bulletForce = 20f;
    [SerializeField] float powerBulletForce = 20f;
    [SerializeField] float buffTime = 3f;

    [Header("Impulse Control")]
    [SerializeField] bool airControl = false;
    [Range(10 , 20)] [SerializeField] public float inAirSpeed = 10f;
    [Range(0, 1)] [SerializeField] float horizontalImpulse = 1f;
    [Range(0, 1)] [SerializeField] float verticalImpulse = 0f;
    [Range(0, 180)] [SerializeField] float minXAngle = 45f;
    [Range(0, 180)] [SerializeField] float maxXAngle = 135f;

    [Header("References")]
    [SerializeField] GameObject gun;
    [SerializeField] public Transform gunEnd;
    [SerializeField] GameObject gunVisuals;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject powerBulletPrefab;
    [SerializeField] public Slider reloadSlider;

    [HideInInspector] Slider ammoSlider;
    [HideInInspector] Camera mainCamera;
    [HideInInspector] public bool powerShotActive = false;
    [HideInInspector] public bool shooting = false;
    [HideInInspector] public bool reloading = false;
    [HideInInspector] public float timer;

    private bool gunFaceRight = true;
    private int currentAmmo;
    private float buffTimeSaved;
    private float aimAngle;
    private Vector2 mousePos;
    private Vector2 direction;
    private Rigidbody2D rb;
    private ParticleSystem cannonParticles;

    public enum ControlMode
    {
        MouseAndKeyboard,
        XboxController
    }

    private void Start()
    {
        ammoSlider = GameObject.Find("Ammo Slider").GetComponent<Slider>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
        cannonParticles = GetComponentInChildren<ParticleSystem>();

        currentAmmo = ammo;
        ammoSlider.maxValue = ammo;
        buffTimeSaved = buffTime;
    }

    private void Update()
    {
        ammoSlider.value = currentAmmo;
        aimAngle = Vector2.Angle(transform.up, direction);
        timer += Time.deltaTime;

        switch (controlMode)
        {
            case ControlMode.MouseAndKeyboard:
                mousePos = Input.mousePosition;
                mousePos = mainCamera.ScreenToWorldPoint(mousePos);
                direction = new Vector2(mousePos.x - gun.transform.position.x, mousePos.y - gun.transform.position.y);
                gun.transform.right = direction;
                break;

            case ControlMode.XboxController:
                direction = new Vector2(Input.GetAxis("Horizontal Aim"), Input.GetAxis("Vertical Aim"));
                gun.transform.right = direction;
                break;
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

        if (powerShotActive)
        {
            buffTime -= Time.deltaTime;

            if (buffTime <= 0)
            {
                buffTime = buffTimeSaved;
                powerShotActive = false;
            }
        }

        if (airControl)
            GetComponent<Scr_PlayerController>().m_AirControl = !shooting;

        if (direction.x > 0 && !gunFaceRight)
            FlipGun();

        else if (direction.x < 0 && gunFaceRight)
            FlipGun();
    }

    public void Shoot(bool powerShot)
    {
        timer = 0;
        currentAmmo -= 1;

        cannonParticles.Play();

        if (powerShot)
        {
            if (currentAmmo > 0)
            {
                Instantiate(powerBulletPrefab, gunEnd.position, gunEnd.rotation);
                ShootingImpulse(powerBulletForce);
            }

            else
                Reload(false);
        }

        else
        {
            if (currentAmmo > 0)
            {
                Instantiate(bulletPrefab, gunEnd.position, gunEnd.rotation);
                ShootingImpulse(bulletForce);
            }

            else
                Reload(false);
        }        
    }

    public void Reload(bool reloadPowerUp)
    {
        if (!reloadPowerUp)
        {
            if (GetComponent<Scr_PlayerController>().m_Grounded)
            {
                reloading = true;
                reloadSlider.gameObject.SetActive(true);
            }
        }

        else
        {
            reloading = true;
            reloadSlider.value = reloadSlider.maxValue;
        }
    }

    void ShootingImpulse(float force)
    {
        if (aimAngle >= minXAngle && aimAngle <= maxXAngle)
        {
            rb.AddForce(-direction.normalized * (force * horizontalImpulse));
        }

        else
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(-direction.normalized * (force * verticalImpulse));
        }
    }

    private void FlipGun()
    {
        gunFaceRight = !gunFaceRight;
        gunVisuals.transform.Rotate(180f, 0f, 0f);
    }
}