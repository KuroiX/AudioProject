using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension
{
    public static Vector2 Position2D(this Transform t)
        => new Vector2(t.position.x, t.position.y);

    public static void SetPostion(this Transform t, float? x = null, float? y = null, float? z = null)
        => t.position = new Vector3(x ?? t.position.x, y ?? t.position.y, z ?? t.position.z);
}

public static class RectExtension
{
    public static void CenterOn(this Rect rect, Vector2 point)
    {
        rect.position = new Vector2(point.x - rect.width / 2, point.y - rect.y / 2);
    }
}

public static class Vector2Extension
{
    public static Vector3 ToVec3(this Vector2 vec2, float z = 0)
        => new Vector3(vec2.x, vec2.y, z);
}