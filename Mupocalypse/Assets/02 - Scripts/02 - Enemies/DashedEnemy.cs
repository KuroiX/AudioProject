using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashedEnemy : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float extraLength;
    [SerializeField]
    private float withdrawSpeed;
    
    private float idleLowness;
    private float extendedLowness;
    private bool withdrawNext;
    private int lives;
    
    void Start()
    {
        idleLowness = transform.position.y;
        extendedLowness = idleLowness - extraLength;
        withdrawNext = false;
        lives = 2;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && withdrawNext == false)
        {
            print("Snap");
            StartCoroutine(Snap());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            print("Withdraw");
            withdrawNext = true;
        }
    }
    
    public void GetDamage()
    {
        lives--;
        if (lives == 0)
        {
            print("Dead");
            Destroy(this.gameObject);
        }
        else
        {
            print("Hit! But there's still one live left.");
        }
    }

    IEnumerator Snap()
    {
        while (transform.position.y > extendedLowness)
        {
            float y = transform.position.y - withdrawSpeed * Time.deltaTime * 8;
            y = y > extendedLowness ? y : extendedLowness;
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
            yield return new WaitForFixedUpdate();
        }
        while (!withdrawNext)
        {
            yield return new WaitForFixedUpdate();
        }
        StartCoroutine(Withdraw());
    }

    IEnumerator Withdraw()
    {
        float y = transform.position.y;
        while (y < idleLowness)
        {
            yield return new WaitForFixedUpdate();
            y += withdrawSpeed * Time.deltaTime;
            y = y < idleLowness ? y : idleLowness;
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
        withdrawNext = false;
    }
}
