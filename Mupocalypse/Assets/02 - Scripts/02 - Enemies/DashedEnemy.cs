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
    private int lives;

    public bool doSnap;

    void Start()
    {
        idleLowness = transform.position.y;
        extendedLowness = idleLowness - extraLength;
        lives = 2;

        doSnap = false;
    }

    void Update()
    {
        if (doSnap)
        {
            StartCoroutine(Snap());
            doSnap = false;
        }
    }
    
    public void GetDamage()
    {
        lives--;
        if (lives == 0)
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator Snap()
    {
        // Snap
        while (transform.position.y > extendedLowness)
        {
            yield return new WaitForFixedUpdate();
            float y = transform.position.y - withdrawSpeed * Time.deltaTime * 8;
            y = y > extendedLowness ? y : extendedLowness;
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
        yield return new WaitForSeconds(1);
        // Return to idle
        while (transform.position.y < idleLowness)
        {
            yield return new WaitForFixedUpdate();
            float y = transform.position.y + withdrawSpeed * Time.deltaTime;
            y = y < idleLowness ? y : idleLowness;
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
    }
}
