using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension
{
    public static Vector2 position2D(this Transform t)
        => new Vector2(t.position.x, t.position.y);
}