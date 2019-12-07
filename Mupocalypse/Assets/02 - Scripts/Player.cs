﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D)),
 RequireComponent(typeof(Animator)),
 RequireComponent(typeof(AudioSource))]
public class Player : Singleton<Player>
{
    // ? the following are 'protected' to avoid unity warnings
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
        [NonSerialized]
        public float initialVelocity;
        public float height;
        public float fallMultiplier;
        public float lowJumpMultiplier;
    }

    [Serializable]
    protected struct DashSettings
    {
        public float speed;
        [Range(0, 1), Tooltip("Factor of the original force")]
        public float slowdown;
    }

    [Serializable]
    protected struct AttackSettings
    {
        public float range;
        public LayerMask layers;
    }

    [Serializable]
    protected struct LivesSettings
    {
        public int lives;
        public int maxLives;
        [Tooltip("Container for the images")]
        public Transform display;
        public Sprite heartFull;
        public Sprite heartEmpty;
    }

    [Serializable]
    protected struct SoundEffects
    {
        public AudioClip attackHit;
        public AudioClip attackMiss;
        public AudioClip dash;
        public AudioClip death;
        public AudioClip jump;
        public AudioClip land;
    }

    [SerializeField]
    protected MoveSettings move;
    [SerializeField]
    protected JumpSettings jump;
    [SerializeField]
    protected DashSettings dash;
    [SerializeField, Tooltip("Red Line shows current range")]
    protected AttackSettings attack;
    [SerializeField, Tooltip("Red Wire-Sphere shows current ground check")]
    protected GroundCheck groundCheck;
    [SerializeField]
    protected LivesSettings lives;
    [SerializeField]
    float timeInvulnerable = .4f;
    [SerializeField, Tooltip("Transform to be turned towards direction.")]
    Transform flip = null;
    [SerializeField]
    protected SoundEffects sfx;

    Rigidbody2D rb;
    Animator animator;
    AudioSource audioSource;
    float moveInput = 0;
    int direction = 1;
    bool grounded;
    Transform platform;
    bool jumpButtonPressed;
    bool sprinting;
    bool canMove = true;
    bool canDash;
    bool dashing;
    bool invulnerable;

    #region MonoBehavior

    private void Start() {
        CalculateJumpVelocity();

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
        if (rb.velocity.y < 0 || rb.velocity.y < 5f)
            rb.velocity += Vector2.up * Physics2D.gravity.y * (jump.fallMultiplier - 1) * Time.fixedDeltaTime;
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

        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * direction * attack.range);
    }

    private void OnValidate() => CalculateJumpVelocity();

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
            if (canMove && (canDash || grounded) && !dashing)
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
        if (lives.lives == 0)
            Die();
        else
        {
            lives.lives--;
            StartCoroutine(Invulnerability());
            UpdateDisplay();
        }
    }

    public void Heal()
    {
        if (lives.lives != lives.maxLives)
        {
            lives.lives++;
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
        var col = Physics2D.OverlapCircle(transform.Position2D() + groundCheck.position, groundCheck.distance, groundCheck.layers);
        if (col != null)
        {
            if (col.gameObject.tag == "Platform") // ?
                platform = col.transform;
            else
                platform = null;
            return true;
        }
        return false;
    }

    void Jump()
    {
        canDash = true;
        rb.velocity += Vector2.up * jump.initialVelocity;
        if (sfx.jump != null)
            audioSource.PlayOneShot(sfx.jump);
        animator.SetTrigger("jump");
        animator.ResetTrigger("hit ground");
    }

    void Dash()
    {
        animator.ResetTrigger("hit ground");
        animator.SetTrigger("dash");
        dashing = true;
        canDash = false;
        rb.velocity += dash.speed * direction * Vector2.right;
        rb.constraints |= RigidbodyConstraints2D.FreezePositionY;
        if (sfx.dash != null)
            audioSource.PlayOneShot(sfx.dash);
    }

    // called from the animation
    void DashEnd()
    {
        dashing = false;
        rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
        rb.velocity -= dash.speed * direction * Vector2.right * dash.slowdown;
        if (Vector2.Dot(rb.velocity, Vector2.right * direction) < 0) // dont move backwards
            rb.velocity = new Vector2(0, rb.velocity.y);
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
        var hit = Physics2D.Raycast(rb.position, Vector2.right * direction, attack.range, attack.layers);
        if (hit)
        {
            var damagable = hit.collider.GetComponent<IDamageable>();
            if (damagable != null)
            {
                damagable.GetDamage();
            }
            animator.SetTrigger("attack");
            canMove = false;
            if (sfx.attackHit != null)
                audioSource.PlayOneShot(sfx.attackHit);
        }
        else if (sfx.attackMiss != null)
            audioSource.PlayOneShot(sfx.attackMiss);
    }

    void Die()
    {
        LevelManager.Instance.FadeOut(GameManager.Instance.savePoint);
        
        Debug.Log("You died!");
        if (sfx.death != null)
            audioSource.PlayOneShot(sfx.death);
    }

    void DirectionFlipped()
    {
        if (flip != null && canMove)
            flip.Rotate(0, 180, 0);
    }

    void Landed()
    {
        animator.SetTrigger("hit ground");
        animator.SetBool("on platform", platform != null);
    }

    void EnableMovement() => canMove = true;
    void DisableMovement() => canMove = false;

    void UpdateDisplay()
    {
        if (lives.display != null)
        {
            var hearts = lives.display.GetComponentsInChildren<Image>();
            var i = 0;
            if (lives.heartFull != null)
                for (; i < Mathf.Min(hearts.Length, lives.lives); i++)
                    hearts[i].sprite = lives.heartFull;
            if (lives.heartEmpty != null)
                for (; i < Mathf.Min(hearts.Length, lives.maxLives); i++)
                    hearts[i].sprite = lives.heartEmpty;
        }
    }

    void CalculateJumpVelocity() => jump.initialVelocity = Mathf.Sqrt(2 * -Physics2D.gravity.y * jump.height);
}
