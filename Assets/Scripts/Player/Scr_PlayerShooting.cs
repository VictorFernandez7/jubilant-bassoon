﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Scr_PlayerShooting : MonoBehaviour
{
    [Header("Input selection")]
    [SerializeField] ControlMode controlMode;

    [Header("Current Gun")]
    [Range(0, 2)] [SerializeField] public int equipedGun;

    [Header("Gun 1: MachineGun")]
    [SerializeField] public int MG_ammo = 10;
    [Range(0, 1)] [SerializeField] public float MG_ShootRate = 1;
    [SerializeField] int MG_reloadSpeed = 2;
    [SerializeField] float MG_bulletForce = 20f;
    [SerializeField] float MG_powerBulletForce = 20f;
    [SerializeField] GameObject MG_bulletPrefab;
    [SerializeField] GameObject MG_powerBulletPrefab;

    [Header("Gun 2: ShotGun")]
    [SerializeField] public int SG_ammo = 10;
    [Range(0, 1)] [SerializeField] public float SG_ShootRate = 1;
    [SerializeField] int SG_reloadSpeed = 2;
    [SerializeField] float SG_bulletForce = 20f;
    [SerializeField] float SG_powerBulletForce = 20f;
    [SerializeField] GameObject SG_bulletPrefab;
    [SerializeField] GameObject SG_powerBulletPrefab;

    [Header("Gun 3: FlameThrower")]
    [SerializeField] public int FT_ammo = 10;
    [Range(0, 1)] [SerializeField] public float FT_ShootRate = 1;
    [SerializeField] int FT_reloadSpeed = 2;
    [SerializeField] float FT_bulletForce = 20f;
    [SerializeField] float FT_powerBulletForce = 20f;
    [SerializeField] GameObject FT_bulletPrefab;
    [SerializeField] GameObject FT_powerBulletPrefab;

    [Header("Impulse Control")]
    [SerializeField] bool airControl = false;
    [Range(10, 20)] [SerializeField] public float inAirSpeed = 10f;
    [Range(0, 1)] [SerializeField] float horizontalImpulse = 1f;
    [Range(0, 1)] [SerializeField] float verticalImpulse = 0f;
    [Range(0, 180)] [SerializeField] float minXAngle = 45f;
    [Range(0, 180)] [SerializeField] float maxXAngle = 135f;
    [SerializeField] public float MaxHorizontalspeed = 10;

    [Header("Power Up Properties")]
    [SerializeField] float buffTime = 3f;

    [Header("References")]
    [SerializeField] GameObject gun;
    [SerializeField] public Transform gunEnd0;
    [SerializeField] public Transform gunEnd1;
    [SerializeField] public Transform gunEnd2;
    [SerializeField] GameObject gunVisuals;
    [SerializeField] public Slider reloadSlider;
    [SerializeField] public ParticleSystem cannonParticles;
    [SerializeField] public ParticleSystem flameParticles;

    [HideInInspector] Slider ammoSlider;
    [HideInInspector] Camera mainCamera;
    [HideInInspector] public bool powerShotActive = false;
    [HideInInspector] public bool shooting = false;
    [HideInInspector] public bool reloading = false;
    [HideInInspector] public int currentAmmo;
    [HideInInspector] public float timer;

    private bool gunFaceRight = true;
    private float buffTimeSaved;
    private float aimAngle;
    private Vector2 mousePos;
    private Vector2 direction;
    private Rigidbody2D rb;

    [SerializeField] public CurrentGun[] currentGuns = new CurrentGun[3];

    public enum ControlMode
    {
        MouseAndKeyboard,
        XboxController
    }

    public struct CurrentGun
    {
        public int Ammo;
        public float ShootRate;
        public int ReloadSpeed;
        public float BulletForce;
        public float PowerBulletForce;
        public GameObject BulletPrefab;
        public GameObject PowerBulletPrefab;
        public Transform GunEnd;

        public CurrentGun (int ammo, float shootRate, int reloadSpeed, float bulletForce, float powerBulletForce, GameObject bulletPrefab, GameObject powerBulletPrefab, Transform gunEnd)
        {
            Ammo = ammo;
            ShootRate = shootRate;
            ReloadSpeed = reloadSpeed;
            BulletForce = bulletForce;
            PowerBulletForce = powerBulletForce;
            BulletPrefab = bulletPrefab;
            PowerBulletPrefab = powerBulletPrefab;
            GunEnd = gunEnd;
        }
    }

    private void Start()
    {
        ammoSlider = GameObject.Find("Ammo Slider").GetComponent<Slider>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();

        currentAmmo = MG_ammo;          // PROVISIONAL
        ammoSlider.maxValue = MG_ammo;  // PROVISIONAL
        buffTimeSaved = buffTime;

        // PROVISIONAL
        //
        currentGuns[0].Ammo = MG_ammo;
        currentGuns[0].ShootRate = MG_ShootRate;
        currentGuns[0].ReloadSpeed = MG_reloadSpeed;
        currentGuns[0].BulletForce = MG_bulletForce;
        currentGuns[0].PowerBulletForce = MG_powerBulletForce;
        currentGuns[0].BulletPrefab = MG_bulletPrefab;
        currentGuns[0].PowerBulletPrefab = MG_powerBulletPrefab;
        currentGuns[0].GunEnd = gunEnd0;
        currentGuns[1].Ammo = SG_ammo;
        currentGuns[1].ShootRate = SG_ShootRate;
        currentGuns[1].ReloadSpeed = SG_reloadSpeed;
        currentGuns[1].BulletForce = SG_bulletForce;
        currentGuns[1].PowerBulletForce = SG_powerBulletForce;
        currentGuns[1].BulletPrefab = SG_bulletPrefab;
        currentGuns[1].PowerBulletPrefab = SG_powerBulletPrefab;
        currentGuns[1].GunEnd = gunEnd1;
        currentGuns[2].Ammo = FT_ammo;
        currentGuns[2].ShootRate = FT_ShootRate;
        currentGuns[2].ReloadSpeed = FT_reloadSpeed;
        currentGuns[2].BulletForce = FT_bulletForce;
        currentGuns[2].PowerBulletForce = FT_powerBulletForce;
        currentGuns[2].BulletPrefab = FT_bulletPrefab;
        currentGuns[2].PowerBulletPrefab = FT_powerBulletPrefab;
        currentGuns[2].GunEnd = gunEnd2;
        //
        ////
    }

    private void Update()
    {
        ammoSlider.value = currentAmmo;
        aimAngle = Vector2.Angle(transform.up, direction);
        timer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangeWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeWeapon(2);

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
            reloadSlider.value += currentGuns[equipedGun].ReloadSpeed * Time.deltaTime;

            if (reloadSlider.value == reloadSlider.maxValue)
            {
                reloading = false;
                reloadSlider.gameObject.SetActive(false);
                currentAmmo = currentGuns[equipedGun].Ammo;
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

        if (Mathf.Abs(rb.velocity.x) > MaxHorizontalspeed)
            rb.velocity = Vector3.right * Mathf.Clamp(rb.velocity.x, -MaxHorizontalspeed, MaxHorizontalspeed) + Vector3.up * rb.velocity.y;

        if (shooting && equipedGun == 2)
            flameParticles.Play();
        else
            flameParticles.Stop();
    }

    public void Shoot(bool powerShot)
    {
        timer = 0;

        if (powerShot)
        {
            if (currentAmmo > 0)
            {
                Instantiate(currentGuns[equipedGun].PowerBulletPrefab, currentGuns[equipedGun].GunEnd.position, currentGuns[equipedGun].GunEnd.rotation);
                ShootingImpulse(currentGuns[equipedGun].PowerBulletForce);

                currentAmmo -= 1;
                cannonParticles.Play();
            }

            else if (!reloading)
                Reload(false);
        }

        else
        {
            if (currentAmmo > 0)
            {
                if (equipedGun!= 2)
                    Instantiate(currentGuns[equipedGun].BulletPrefab, currentGuns[equipedGun].GunEnd.position, currentGuns[equipedGun].GunEnd.rotation);

                ShootingImpulse(currentGuns[equipedGun].BulletForce);

                currentAmmo -= 1;

                if (equipedGun != 2)
                    cannonParticles.Play();
            }

            else if (!reloading)
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

    public void ChangeWeapon(int targetWeapon)
    {
        currentAmmo = currentGuns[targetWeapon].Ammo;
        equipedGun = targetWeapon;
        ammoSlider.maxValue = currentGuns[equipedGun].Ammo;
    }
}