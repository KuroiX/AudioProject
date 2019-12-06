using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Singleton<Player>
{
    [Serializable]
    protected struct GroundCheck
    {
        public Vector2 position;
        public float distance;
        public LayerMask layers;
    }

    [Serializable]
    protected struct MoveSettings
    {
        public float speed;
        public float sprintMultiplyer;
    }

    [Serializable]
    protected struct JumpSettings
    {
        public float initialVelocity;
        public float fallMultiplyer;
        public float lowJumpMultiplyer;
    }

    [SerializeField]
    protected MoveSettings move;
    [SerializeField]
    protected JumpSettings jump;
    [SerializeField]
    protected GroundCheck groundCheck;

    float moveInput = 0;
    Rigidbody2D rb;
    bool grounded;
    bool jumpButtonPressed;
    bool sprinting;

    #region MonoBehavior

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (IsGrounded())
        {

        }
        else grounded = false;

        // apply additional gravity
        if (rb.velocity.y < 0)
            rb.velocity += Vector2.up * Physics.gravity.y * (jump.fallMultiplyer - 1) * Time.fixedDeltaTime;
        else if (rb.velocity.y > 0 && !jumpButtonPressed)
            rb.velocity += Vector2.up * Physics.gravity.y * (jump.lowJumpMultiplyer - 1) * Time.fixedDeltaTime;

        Move();
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        var pos = transform.position + new Vector3(groundCheck.position.x, groundCheck.position.y, 0);
        Gizmos.DrawLine(pos, pos - transform.up * groundCheck.distance);
        Gizmos.DrawWireSphere(pos, groundCheck.distance);
    }

    #endregion
    #region Input

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<float>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpButtonPressed = true;
            if (grounded)
            {
                rb.velocity += Vector2.up * jump.initialVelocity;
            }
        }
        else if (context.canceled)
            jumpButtonPressed = false;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started)
            sprinting = true;
        else if (context.canceled)
            sprinting = false;
    }

    #endregion

    protected bool IsGrounded()
    {
        var cols = Physics2D.OverlapCircle(transform.position2D() + groundCheck.position, groundCheck.distance, groundCheck.layers);
        return cols != null;
    }

    void Move()
    {
        var v = moveInput * Vector2.right * Time.fixedDeltaTime * move.speed;
        if (sprinting)
            v *= move.sprintMultiplyer;
        rb.position += v;
    }

    void Landed()
    {
        // TODO
    }
}
