using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Scr_PlayerController))]
[RequireComponent(typeof(Scr_PlayerHealth))]
[RequireComponent(typeof(Scr_PlayerInput))]
public class Scr_PlayerShooting : MonoBehaviour
{
    [Header("Input selection")]
    [SerializeField] ControlMode controlMode;

    [Header("Current Gun")]
    [Range(0, 2)] [SerializeField] public int equipedGun;

    [Header("Gun 0")]
    [SerializeField] private string MG_gunName = "MachineGun";
    [SerializeField] private Sprite MG_gunSprite;
    [Range(5, 100)] [SerializeField] public float MG_damage = 25;
    [SerializeField] public int MG_ammo = 10;
    [Range(0, 1)] [SerializeField] public float MG_shootRate = 1;
    [SerializeField] private int MG_reloadSpeed = 2;
    [SerializeField] private float MG_bulletForce = 20f;
    [SerializeField] private float MG_powerBulletForce = 20f;
    [SerializeField] private GameObject MG_bulletPrefab;
    [SerializeField] private GameObject MG_powerBulletPrefab;

    [Header("Gun 1")]
    [SerializeField] private string SG_gunName = "ShotGun";
    [SerializeField] private Sprite SG_gunSprite;
    [Range(5, 100)] [SerializeField] public float SG_damage = 50;
    [SerializeField] public int SG_ammo = 10;
    [Range(0, 1)] [SerializeField] public float SG_shootRate = 1;
    [SerializeField] private int SG_reloadSpeed = 2;
    [SerializeField] private float SG_bulletForce = 20f;
    [SerializeField] private float SG_powerBulletForce = 20f;
    [SerializeField] private GameObject SG_bulletPrefab;
    [SerializeField] private GameObject SG_powerBulletPrefab;

    [Header("Gun 2")]
    [SerializeField] private string FT_gunName = "FlameThrower";
    [SerializeField] private Sprite FT_gunSprite;
    [Range(5, 100)] [SerializeField] public float FT_damage = 5;
    [SerializeField] public int FT_ammo = 10;
    [Range(0, 1)] [SerializeField] public float FT_shootRate = 1;
    [SerializeField] private int FT_reloadSpeed = 2;
    [SerializeField] private float FT_bulletForce = 20f;
    [SerializeField] private float FT_powerBulletForce = 20f;
    [SerializeField] private GameObject FT_bulletPrefab;
    [SerializeField] private GameObject FT_powerBulletPrefab;

    [Header("Impulse Control")]
    [SerializeField] private bool airControl = false;
    [Range(10, 20)] [SerializeField] public float inAirSpeed = 10f;
    [Range(0, 1)] [SerializeField] private float horizontalImpulse = 1f;
    [Range(0, 1)] [SerializeField] private float verticalImpulse = 0f;
    [Range(0, 180)] [SerializeField] private float minXAngle = 45f;
    [Range(0, 180)] [SerializeField] private float maxXAngle = 135f;
    [SerializeField] public float MaxHorizontalspeed = 10;

    [Header("Power Up Properties")]
    [SerializeField] private float buffTime = 3f;

    [Header("References")]
    [SerializeField] private GameObject gun;
    [SerializeField] public Transform gunEnd0;
    [SerializeField] public Transform gunEnd1;
    [SerializeField] public Transform gunEnd2;
    [SerializeField] private GameObject gunVisuals;
    [SerializeField] private GameObject playerVisuals;
    [SerializeField] public Slider reloadSlider;
    [SerializeField] public ParticleSystem cannonParticles;
    [SerializeField] public ParticleSystem flameParticles;
    [SerializeField] private TextMeshProUGUI gunText;
    [SerializeField] private SpriteRenderer gunSprite;
    [SerializeField] private GameObject reloadText;

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
    private Animator anim;
    private Slider ammoSlider;
    private Camera mainCamera;

    public CurrentGun[] currentGuns = new CurrentGun[3];

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
        public string GunName;
        public Sprite GunSprite;

        public CurrentGun (int ammo, float shootRate, int reloadSpeed, float bulletForce, float powerBulletForce, GameObject bulletPrefab, GameObject powerBulletPrefab, Transform gunEnd, string gunName, Sprite gunSprite)
        {
            Ammo = ammo;
            ShootRate = shootRate;
            ReloadSpeed = reloadSpeed;
            BulletForce = bulletForce;
            PowerBulletForce = powerBulletForce;
            BulletPrefab = bulletPrefab;
            PowerBulletPrefab = powerBulletPrefab;
            GunEnd = gunEnd;
            GunName = gunName;
            GunSprite = gunSprite;
        }
    }

    private void Start()
    {
        ammoSlider = GameObject.Find("Ammo Slider").GetComponent<Slider>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        currentAmmo = MG_ammo;          // PROVISIONAL
        ammoSlider.maxValue = MG_ammo;  // PROVISIONAL
        buffTimeSaved = buffTime;

        /// PROVISIONAL
        //
        currentGuns[0].Ammo = MG_ammo; 
        currentGuns[0].ShootRate = MG_shootRate;
        currentGuns[0].ReloadSpeed = MG_reloadSpeed;
        currentGuns[0].BulletForce = MG_bulletForce;
        currentGuns[0].PowerBulletForce = MG_powerBulletForce;
        currentGuns[0].BulletPrefab = MG_bulletPrefab;
        currentGuns[0].PowerBulletPrefab = MG_powerBulletPrefab;
        currentGuns[0].GunEnd = gunEnd0;
        currentGuns[0].GunName = MG_gunName;
        currentGuns[0].GunSprite = MG_gunSprite;
        currentGuns[1].Ammo = SG_ammo;
        currentGuns[1].ShootRate = SG_shootRate;
        currentGuns[1].ReloadSpeed = SG_reloadSpeed;
        currentGuns[1].BulletForce = SG_bulletForce;
        currentGuns[1].PowerBulletForce = SG_powerBulletForce;
        currentGuns[1].BulletPrefab = SG_bulletPrefab;
        currentGuns[1].PowerBulletPrefab = SG_powerBulletPrefab;
        currentGuns[1].GunEnd = gunEnd1;
        currentGuns[1].GunName = SG_gunName;
        currentGuns[1].GunSprite = SG_gunSprite;
        currentGuns[2].Ammo = FT_ammo;
        currentGuns[2].ShootRate = FT_shootRate;
        currentGuns[2].ReloadSpeed = FT_reloadSpeed;
        currentGuns[2].BulletForce = FT_bulletForce;
        currentGuns[2].PowerBulletForce = FT_powerBulletForce;
        currentGuns[2].BulletPrefab = FT_bulletPrefab;
        currentGuns[2].PowerBulletPrefab = FT_powerBulletPrefab;
        currentGuns[2].GunEnd = gunEnd2;
        currentGuns[2].GunName = FT_gunName;
        currentGuns[2].GunSprite = FT_gunSprite;
        //
        ///
        
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

        if (currentAmmo <= 0)
            reloadText.SetActive(true);
        else if (!reloading)
            reloadText.SetActive(false);
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

    private void ShootingImpulse(float force)
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
        playerVisuals.GetComponent<SpriteRenderer>().flipX = !playerVisuals.GetComponent<SpriteRenderer>().flipX;
    }

    public void ChangeWeapon(int targetWeapon)
    {
        currentAmmo = currentGuns[targetWeapon].Ammo;
        equipedGun = targetWeapon;
        ammoSlider.maxValue = currentGuns[equipedGun].Ammo;
        gunText.text = currentGuns[equipedGun].GunName;
        anim.SetTrigger("WeaponChange");
        gunSprite.sprite = currentGuns[targetWeapon].GunSprite;
    }
}