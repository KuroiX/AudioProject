﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
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
        public float sprintMultiplier;
    }

    [Serializable]
    protected struct JumpSettings
    {
        public float initialVelocity;
        public float fallMultiplier;
        public float lowJumpMultiplier;
    }

    [SerializeField]
    protected MoveSettings move;
    [SerializeField]
    protected JumpSettings jump;
    [SerializeField]
    protected GroundCheck groundCheck;
    [SerializeField]
    float dashSpeed = 10;
    [SerializeField]
    int lives = 3;
    [SerializeField]
    int maxLives = 3;
    [SerializeField]
    float timeInvulnerable = .4f;
    [SerializeField]
    Text livesDisplay = null;
    [SerializeField]
    Transform flip = null;

    Rigidbody2D rb;
    Animator animator;
    AudioSource audioSource;
    float moveInput = 0;
    int direction = 1;
    bool grounded;
    bool jumpButtonPressed;
    bool sprinting;
    bool canMove = true;
    bool canDash;
    bool dashing;
    bool invulnerable;

    #region MonoBehavior

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();

        UpdateDisplay();
    }

    private void FixedUpdate()
    {
        if (IsGrounded())
        {
            if (!grounded)
                Landed();
            grounded = true;
        }
        else grounded = false;

        // apply gravity
        if (rb.velocity.y < 0)
            rb.velocity += Vector2.up * Physics.gravity.y * (jump.fallMultiplier - 1) * Time.fixedDeltaTime;
        // else if (rb.velocity.y > 0 && !jumpButtonPressed)
        //     rb.velocity += Vector2.up * Physics.gravity.y * (jump.lowJumpMultiplier - 1) * Time.fixedDeltaTime;

        if (canMove)
            Move();
    }

    private void OnCollisionEnter2D(Collision2D other) {
        // ! Test code
        switch (other.collider.tag)
        {
            case "Enemy":
                Damage();
                break;
            case "Pickup":
                Heal();
                break;
        }
        // ! end
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        var pos = transform.position + new Vector3(groundCheck.position.x, groundCheck.position.y, 0);
        Gizmos.DrawWireSphere(pos, groundCheck.distance);

        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * direction);
    }

    #endregion
    #region Input

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<float>();
        if (moveInput != 0)
        {
            var d = direction;
            if (moveInput > 0)
                d = 1;
            else
                d = -1;
            if (d != direction)
                DirectionFlipped();
            direction = d;
            animator.SetBool("walking", true);
        }
        else
            animator.SetBool("walking", false);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpButtonPressed = true;
            if (grounded && canMove && !dashing)
                Jump();
        }
        else if (context.canceled)
            jumpButtonPressed = false;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started)
            if ((canDash || grounded) && !dashing)
                Dash();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started)
            sprinting = true;
        else if (context.canceled)
            sprinting = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
            Attack();
    }

    #endregion
    #region Public

    public void Damage()
    {
        if (invulnerable) return;
        if (lives == 0)
            Die();
        else
        {
            lives--;
            StartCoroutine(Invulnerability());
            UpdateDisplay();
        }
    }

    public void Heal()
    {
        if (lives != maxLives)
        {
            lives++;
            UpdateDisplay();
        }
    }

    #endregion

    IEnumerator Invulnerability()
    {
        invulnerable = true;
        animator.SetBool("invulnerable", true);
        yield return new WaitForSeconds(timeInvulnerable);
        invulnerable = false;
        animator.SetBool("invulnerable", false);
    }

    bool IsGrounded()
    {
        var cols = Physics2D.OverlapCircle(transform.position2D() + groundCheck.position, groundCheck.distance, groundCheck.layers);
        return cols != null;
    }

    void Jump()
    {
        canDash = true;
        rb.velocity += Vector2.up * jump.initialVelocity;
        audioSource.Play();
        animator.SetTrigger("jump");
        animator.ResetTrigger("hit ground");
    }

    void Dash()
    {
        animator.ResetTrigger("hit ground");
        animator.SetTrigger("dash");
        dashing = true;
        canDash = false;
        rb.velocity += dashSpeed * direction * Vector2.right;
        rb.constraints |= RigidbodyConstraints2D.FreezePositionY;
    }

    void DashEnd()
    {
        dashing = false;
        rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
    }

    void Move()
    {
        var v = moveInput * Vector2.right * Time.fixedDeltaTime * move.speed;
        if (sprinting)
            v *= move.sprintMultiplier;
        rb.position += v;
    }

    void Attack()
    {
        // TODO
    }

    void Die()
    {
        Debug.Log("You died!");
    }

    void DirectionFlipped()
    {
        if (flip != null && canMove)
            flip.Rotate(0, 180, 0);
    }

    void Landed()
    {
        animator.SetTrigger("hit ground");
    }

    void EnableMovement() => canMove = true;
    void DisableMovement() => canMove = false;

    void UpdateDisplay()
    {
        if (livesDisplay != null)
            livesDisplay.text = string.Format("Lives: {0}", lives);
    }
}
