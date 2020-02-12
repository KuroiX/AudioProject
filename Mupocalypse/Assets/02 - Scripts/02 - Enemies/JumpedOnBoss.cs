using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class JumpedOnBoss : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float jumpSpeed;
    [SerializeField]
    private float variance;
    [SerializeField]
    private float jumpHeight;
    [SerializeField]
    private float jumpDuration;
    [SerializeField]
    private int jumpOdd;
    [SerializeField]
    private int attackOdd;
    [SerializeField]
    private float timeInvulnerable = .3f;
    [SerializeField]
    private GameObject drop;
    [SerializeField]
    private float bounce = .6f;

    private float leftBorder;
    private float rightBorder;
    private float groundHeight;

    private bool goRight;
    private int lives;
    private float jumping;
    private bool attacking;

    private bool invulnerable;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        leftBorder = transform.position.x - 2 * variance;
        rightBorder = transform.position.x;
        groundHeight = transform.position.y;
        goRight = false;
        lives = 3;
        jumping = 0;
        attacking = false;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        StartCoroutine(Move());

        drop.SetActive(false);
    }

    void IDamageable.GetDamage()
    {
        // TODO: Maybe add condition
        LoseLive();
    }

    void LoseLive()
    {
        if (!invulnerable)
        {
            lives -= 1;
            if (lives == 0)
            {
                //TODO: right boss music
                if (SceneManager.GetActiveScene().buildIndex == 4)
                {
                    AudioManager.Instance.current = AudioManager.Instance.audioClips[2];
                    AudioManager.Instance.StartFade(0.4f, AudioManager.Instance.current);
                }
                else
                {
                    AudioManager.Instance.current = AudioManager.Instance.audioClips[5];
                    AudioManager.Instance.StartFade(0.4f, AudioManager.Instance.current);
                }
                
                //AudioManager.Instance.sources[1].PlayOneShot(death);

                if (drop != null)
                {
                    drop.SetActive(true);
                    drop.transform.position = Vector3.zero;
                }

                Destroy(this.gameObject);
                ProgressManager.Instance.defeatedBosses.Add(SceneManager.GetActiveScene().buildIndex, true);
                /*GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
                foreach (GameObject go in doors)
                {
                    go.GetComponent<Door>().UnLock();
                }*/
            }
            else
            {
                // print("Lost a live: " + lives + "left");
                StartCoroutine(LoseLifeEnd());
            }
        }
    }

    IEnumerator LoseLifeEnd()
    {
        animator.SetTrigger("invincible");
        invulnerable = true;
        yield return new WaitForSeconds(timeInvulnerable);
        animator.SetTrigger("invincible");
        invulnerable = false;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        GameObject otherObject = col.gameObject;
        if (otherObject.CompareTag("Player"))
        {
            float x = Mathf.Abs(transform.position.x - otherObject.transform.position.x);
            float y = Mathf.Abs(transform.position.y - otherObject.transform.position.y);

            if (x <= y*10 && transform.position.y - 1 < otherObject.transform.position.y)
            {
                if (attacking)
                {
                    // TODO: Player can walk on Boss regularly/gets thrown off
                    Player.Instance.Jump(bounce);
                }
                else
                {
                    LoseLive();
                    source.PlayOneShot(hit);
                    Player.Instance.Jump(bounce);
                }
            }
            else
            {
                col.gameObject.GetComponent<Player>().Damage();
                // TODO: Maybe let player bounce off
            }
        }
    }

    void Turn()
    {
        goRight = !goRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
        SetRandomJumps();
        SetRandomAttacks();
    }
    
    void SetRandomJumps()
    {
        int rand = Random.Range(0, jumpOdd);
        if (rand == 0)
        {
            jumping = jumpDuration;
            source.PlayOneShot(jump);
            animator.SetInteger("State", 1);
        }
        else
        {
            animator.SetInteger("State", 0);
        }
    }

    void SetRandomAttacks()
    {
        int rand = Random.Range(0, attackOdd);
        if (rand == 0)
        {
            attacking = true;
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        animator.SetInteger("State", 2);
        PlayFirstPart();
        yield return new WaitForSeconds(1);
        attacking = false;
        if (jumping > 0)
        {
            animator.SetInteger("State", 1);
        }
        else
        {
            animator.SetInteger("State", 0);
        }
    }

    IEnumerator Move()
    {
        while (lives > 0)
        {
            // Walking
            float distance = walkSpeed * Time.deltaTime;
            if (jumping > 0)
            {
                distance *= jumpSpeed;
            }
            float x = transform.position.x;
            if (goRight)
            {
                x += distance;
                if (x >= rightBorder)
                {
                    Turn();
                    while(attacking)
                    {
                        yield return new WaitForFixedUpdate();
                    }
                }
            }
            else
            {
                x -= distance;
                if (x <= leftBorder)
                {
                    Turn();
                    while (attacking)
                    {
                        yield return new WaitForFixedUpdate();
                    }
                }
            }

            // Jumping
            float y = groundHeight;
            if (jumping > 0)
            {
                y += Mathf.Sin(jumping * Mathf.PI) * jumpHeight;
                jumping -= Time.deltaTime;
                if (jumping <= 0)
                {
                    animator.SetInteger("State", 0);
                }
            }

            transform.position = new Vector3(x, y, transform.position.z);
            yield return new WaitForFixedUpdate();
        }
    }

    #region audio

    public AudioClip attack1;
    public AudioClip attack2;
    public AudioClip hit;
    public AudioClip jump;
    //public AudioClip death;
    public AudioSource source;

    public void PlayFirstPart()
    {
        source.PlayOneShot(attack1);
    }
    
    //Called from animation
    public void PlaySecondPart()
    {
        source.PlayOneShot(attack2);
    }
    
    #endregion
}
