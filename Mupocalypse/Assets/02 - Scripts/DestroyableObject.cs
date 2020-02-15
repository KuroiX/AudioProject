using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour
{
    public void GetDestroyed()
    {
        Destroy(this.gameObject);
    }

}
