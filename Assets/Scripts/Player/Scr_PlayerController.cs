using UnityEngine;
using UnityEngine.Events;

public class Scr_PlayerController : MonoBehaviour
{
    [Header("Movement Parameters")]
    [Range(0, 100)] [SerializeField] public float runSpeed = 40f;
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    [SerializeField] private LayerMask m_WhatIsGround;

    [Header("References")]
    [SerializeField] private Transform m_GroundCheck;
    [SerializeField] private Transform m_CeilingCheck;
    [SerializeField] private Collider2D m_CrouchDisableCollider;

    [HideInInspector] public bool m_AirControl = false;
    [HideInInspector] public bool m_Grounded;
    [HideInInspector] public bool m_FacingRight = true;

    const float k_GroundedRadius = .2f;
    const float k_CeilingRadius = .2f;
    private Rigidbody2D m_Rigidbody2D;
    private Vector3 m_Velocity = Vector3.zero;
    private bool m_wasCrouching = false;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
            }
        }
    }

    public void Move(float move, bool crouch)
    {
        if (!crouch)
        {
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                crouch = true;
        }

        if (m_Grounded || m_AirControl)
        {
            Vector3 targetVelocity = new Vector2(move * GetComponent<Scr_PlayerShooting>().inAirSpeed, m_Rigidbody2D.velocity.y);
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            if (crouch)
            {
                move *= m_CrouchSpeed;

                if (!m_wasCrouching)
                    m_wasCrouching = true;
                
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }

            else
            {
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                    m_wasCrouching = false;
            }
            
            if (move > 0 && !m_FacingRight)
                Flip();

            else if (move < 0 && m_FacingRight)
                Flip();
        }
    }
    
    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}