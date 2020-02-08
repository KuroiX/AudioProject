using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

using UnityEngine.SceneManagement;
using Random = System.Random;

[RequireComponent(typeof(Rigidbody2D)),
 RequireComponent(typeof(Animator)),
 RequireComponent(typeof(AudioSource))]
public class Player : Singleton<Player> {
    public enum Ability
    {
        dash,
        jump,
        attack,
        sprint
    }

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
        [NonSerialized]
        public Transform display;
        [Tooltip("Container for the images")]
        public string displayTagName;
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
        public AudioClip hit;
        public AudioClip jump;
        public AudioClip land;
        public AudioClip unlock;
    }

    [Serializable]
    protected struct NotesSettings
    {
        [NonSerialized]
        public Animator notes;
        [Tooltip("Container for the notes")]
        public string notesTag;
        public string dashTrigger;
        public string attackTrigger;
        public string jumpTrigger;
        public string sprintTrigger;
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
    SpriteRenderer flip = null;
    [SerializeField]
    protected SoundEffects sfx;
    [SerializeField]
    //protected NotesSettings notes;

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

    // Abilities unlocked
    bool jumpUnlocked;
    bool dashUnlocked;
    bool attackUnlocked;
    bool sprintUnlocked;
    private static readonly int Jump1 = Animator.StringToHash("jump");

    public void DisableAbilities()
    {
        jumpUnlocked = false;
        dashUnlocked = false;
        attackUnlocked = false;
        sprintUnlocked = false;
    }

    #region Singleton

    protected override void OnEnableCallback()
    {
        SceneManager.sceneLoaded += OnLoad;
    }

    #endregion
    #region MonoBehavior

    private void Start() {
        CalculateJumpVelocity();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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
            /*case "Enemy":
                Damage();
                break;*/
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

    public bool paused;
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<float>();
        if (moveInput != 0 && !paused)
        {
            var d = direction;
            if (moveInput > 0)
            {
                d = 1;
                flip.flipX = false;
            }

            else
            {
                d = -1;
                flip.flipX = true;
            }
                
            //if (d != direction)
                //DirectionFlipped();
            direction = d;
            animator.SetBool("walking", true);
        }
        else
            animator.SetBool("walking", false);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && jumpUnlocked && !paused)
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
        if (context.started && dashUnlocked && !paused)
            if (canMove && (canDash || grounded) && !dashing)
                Dash();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started && sprintUnlocked && !paused)
            sprinting = true;
        else if (context.canceled)
            sprinting = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && attackUnlocked && !paused)
            Attack();
    }

    #endregion
    #region Public

    public void Damage()
    {
        if (invulnerable) return;
        
        rb.AddForce(Vector2.left*(direction > 0 ? 500: -500));
        audioSource.PlayOneShot(sfx.hit);
        
        if (lives.lives == 1)
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

    public void IncreaseMaxHealth()
    {
        lives.maxLives++;
        UpdateDisplay();
    }

    public void UnlockAbility(Ability ability)
    {
        switch (ability)
        {
            case Ability.attack:
                attackUnlocked = true;
                //notes.notes.SetTrigger(notes.attackTrigger);
                break;
            case Ability.jump:
                jumpUnlocked = true;
                //notes.notes.SetTrigger(notes.jumpTrigger);
                break;
            case Ability.dash:
                dashUnlocked = true;
                //notes.notes.SetTrigger(notes.dashTrigger);
                break;
            case Ability.sprint:
                sprintUnlocked = true;
                //notes.notes.SetTrigger(notes.sprintTrigger);
                break;
        }
        audioSource.PlayOneShot(sfx.unlock);
    }

    public void Jump(float factor = 1)
    {
        /*if (rb.velocity.y > 0)
        {
            return;
        }*/
        
        if (rb.velocity.y > 0 && platform != null)
        {
            GetComponent<TrailRenderer>().emitting = true;
        }
        
        canDash = true;
        rb.velocity += Vector2.up * jump.initialVelocity * factor;
        if (sfx.jump != null)
            audioSource.PlayOneShot(sfx.jump);
        animator.SetTrigger(Jump1);
        animator.ResetTrigger("hit ground");
    }
    
    void UltraBoost() {
    
    }

    #endregion

    private void OnLoad(Scene scene, LoadSceneMode mode)
    {
        lives.display = GameObject.FindGameObjectWithTag(lives.displayTagName)?.transform;
        /*notes.notes = GameObject.FindGameObjectWithTag(notes.notesTag)?.GetComponent<Animator>();
        if (attackUnlocked)
            notes.notes.SetTrigger(notes.attackTrigger);
        if (jumpUnlocked)
            notes.notes.SetTrigger(notes.jumpTrigger);
        if (sprintUnlocked)
            notes.notes.SetTrigger(notes.sprintTrigger);
        if (dashUnlocked)
            notes.notes.SetTrigger(notes.dashTrigger);*/
        UpdateDisplay();
    }

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

    void Dash()
    {
        animator.ResetTrigger("hit ground");
        animator.SetTrigger("dash");
        dashing = true;
        canDash = false;
        transform.position += Vector3.up * 0.05f;
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

    #region Moonwalk
    private bool isAttacking;
    private bool flipped;

    public bool moonwalkUnlocked = true;
    private bool isMoonwalking;
    
    public void FlipAfterAttack()
    {
        isAttacking = false;
        if (flipped && IsGrounded())
        {
            /*if (moonwalkUnlocked && !isMoonwalking)
            {
                isMoonwalking = true;
                StartCoroutine(Moonwalk());
            }
            else
            {*/
                Debug.Log("hier?");
                DirectionFlipped();
                flipped = false;
            //}
        }

        /*isAttacking = false;
        if (flipped)
        {
            //Debug.Log("Hello");
            DirectionFlipped();
            flipped = false;
        }*/
    }

    IEnumerator Moonwalk()
    {
        Debug.Log("moonwalk started");
        yield return new WaitForSeconds(2);
        //Debug.Log("Hello");
        DirectionFlipped();
        flipped = false;
        isMoonwalking = false;
        Debug.Log("moonwalk end");
    }

    
    
    #endregion

    public void CheckAttack()
    {
        var hit = Physics2D.Raycast(rb.position, Vector2.right * direction, attack.range, attack.layers);
        var hit2 = Physics2D.Raycast(rb.position+new Vector2(0, 0.5f), Vector2.right * direction, attack.range, attack.layers);
        var hit3 = Physics2D.Raycast(rb.position-new Vector2(0, 0.5f), Vector2.right * direction, attack.range, attack.layers);

        if (hit)
        {
            GameObject destroyable = hit.collider.gameObject;
            if (destroyable.CompareTag("DestroyableObject"))
            {
                destroyable.GetComponent<DestroyableObject>().GetDestroyed();
            }

            var damagable = hit.collider.GetComponent<IDamageable>();
            if (damagable != null)
            {
                damagable.GetDamage();
            }
        } else if (hit2)
        {
            GameObject destroyable = hit2.collider.gameObject;
            if (destroyable.CompareTag("DestroyableObject"))
            {
                destroyable.GetComponent<DestroyableObject>().GetDestroyed();
            }

            var damagable = hit2.collider.GetComponent<IDamageable>();
            if (damagable != null)
            {
                damagable.GetDamage();
            }
        } else if (hit3)
        {
            GameObject destroyable = hit3.collider.gameObject;
            if (destroyable.CompareTag("DestroyableObject"))
            {
                destroyable.GetComponent<DestroyableObject>().GetDestroyed();
            }

            var damagable = hit3.collider.GetComponent<IDamageable>();
            if (damagable != null)
            {
                damagable.GetDamage();
            }
        }
        //else if (sfx.attackMiss != null)
        //    audioSource.PlayOneShot(sfx.attackMiss);
    }
    void Attack()
    {
        if (!isMoonwalking && !isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("attack");
            if (grounded) canMove = false;
            if (sfx.attackHit != null)
                audioSource.PlayOneShot(sfx.attackHit);
            CheckAttack();
        }
    }

    void Die()
    {
        GameManager.Instance.spawnID = -1;
        LevelManager.Instance.FadeOut(GameManager.Instance.savePoint);
        
        Debug.Log("You died!");
        if (sfx.death != null)
            audioSource.PlayOneShot(sfx.death);

        lives.lives = lives.maxLives;

        //SceneManager.LoadScene("ImmedialtelyGoBackScene");
    }

    void DirectionFlipped()
    {
        if (flip != null && canMove)
        {
            flip.flipX = true;
        }
        //Debug.Log("DirectionFlipped");
        // For moonwalking
        if (isAttacking && !flipped && IsGrounded())
        {
            flipped = true;
        }
        else
        {
            flipped = false;
        }
    }

    void Landed()
    {
        GetComponent<TrailRenderer>().emitting = false;
        animator.SetTrigger("hit ground");
        animator.SetBool("on platform", platform != null);
    }

    void EnableMovement() => canMove = true;
    void DisableMovement() => canMove = false;

    void UpdateDisplay()
    {
        if (lives.display != null)
        {
            var hearts = lives.display.GetComponentsInChildren<Image>(true);
            var i = 0;
            if (lives.heartFull != null)
            {
                for (; i < Mathf.Min(hearts.Length, lives.lives); i++)
                {
                    hearts[i].gameObject.SetActive(true);
                    hearts[i].sprite = lives.heartFull;
                }
            }
            if (lives.heartEmpty != null)
            {
                for (; i < Mathf.Min(hearts.Length, lives.maxLives); i++)
                {
                    hearts[i].gameObject.SetActive(true);
                    hearts[i].sprite = lives.heartEmpty;
                }
            }
            for (; i < hearts.Length; i++)
                hearts[i].gameObject.SetActive(false);
        }
    }

    void CalculateJumpVelocity() => jump.initialVelocity = Mathf.Sqrt(2 * -Physics2D.gravity.y * jump.height);
}
