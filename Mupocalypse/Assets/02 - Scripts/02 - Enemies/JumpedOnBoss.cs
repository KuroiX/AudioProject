using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpedOnBoss : MonoBehaviour
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
    private int odd;

    private float leftBorder;
    private float rightBorder;
    private float groundHeight;

    private bool goRight;
    private int lives;
    private float jumping;

    void Start()
    {
        leftBorder = transform.position.x - variance;
        rightBorder = transform.position.x + variance;
        groundHeight = transform.position.y;
        goRight = true;
        lives = 3;
        jumping = 1;

        StartCoroutine(Move());
    }

    void Update()
    {

    }

    void Turn()
    {
        goRight = !goRight;
        SetRandomJumps();
    }
    
    void SetRandomJumps()
    {
        int rand = Random.Range(0, odd);
        if (rand == 0)
        {
            jumping = 1;
        }
    }

    IEnumerator Move()
    {
        while (lives > 0)
        {
            // walking
            float distance = walkSpeed * Time.deltaTime;
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

            // jumping
            float y = transform.position.y;
            if (jumping > 0)
            {
                y = Mathf.Sin(jumping * Mathf.PI) * jumpHeight;
                jumping -= jumpSpeed * Time.deltaTime;
            }

            transform.position = new Vector3(x, y, transform.position.z);
            yield return new WaitForFixedUpdate();
        }
        // Dead
    }
}
