using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Time.timeScale = 0.5f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 1f;
        }
    }
}
