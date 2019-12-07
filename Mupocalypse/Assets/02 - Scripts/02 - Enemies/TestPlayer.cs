using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [SerializeField]
    private float speed;

    void Update()
    {
        float x = transform.position.x + Input.GetAxis("Horizontal") * speed;
        float y = transform.position.y + Input.GetAxis("Vertical") * speed;
        transform.position = new Vector3(x, y, transform.position.z);
    }
}
