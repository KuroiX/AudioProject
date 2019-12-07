using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpedOnEnemy : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float variance;
    private float leftBorder;
    private float rightBorder;
    private bool goRight;
    private bool alive;

    private SpriteRenderer spriteRenderer;
    private Collider2D collider2d;
    
    void Start()
    {
        leftBorder = transform.position.x - variance;
        rightBorder = transform.position.x + variance;
        goRight = true;
        alive = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2d = GetComponent<Collider2D>(); ;

        StartCoroutine(Walk());
    }
    
    void Die()
    {
        print("Dead");
        Destroy(this.gameObject);
    }

    void OnCollisionEnter2D (Collision2D col)
    {
        GameObject otherObject = col.gameObject;
        if (otherObject.CompareTag("Player"))
        {
            float x = Mathf.Abs(transform.position.x - otherObject.transform.position.x);
            float y = Mathf.Abs(transform.position.y - otherObject.transform.position.y);

            if (x <= y && transform.position.y < otherObject.transform.position.y)
            {
                Die();
                // TODO: Maybe jump
            }
            else
            {
                print("Hit");
                // TODO: Hit player
            }
        }
    }

    void Turn()
    {
        goRight = !goRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
        if (goRight)
        {
            collider2d.offset = new Vector2(-collider2d.offset.x, collider2d.offset.y);
        }
        else
        {
            collider2d.offset = new Vector2(-collider2d.offset.x, collider2d.offset.y);
        }
    }

    IEnumerator Walk()
    {
        while (alive)
        {
            float distance = speed * Time.deltaTime;
            float x = transform.position.x;
            if (goRight)
            {
                x += distance;
                if (x >= rightBorder)
                {
                    Turn();
                }
            }
            else
            {
                x -= distance;
                if (x <= leftBorder)
                {
                    Turn();
                }
            }
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
            yield return new WaitForFixedUpdate();
        }
    }
}
