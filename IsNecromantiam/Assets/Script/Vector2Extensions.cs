using UnityEngine;

public static class Vector2Extensions
{
    public static Vector3 xz(this Vector2 v) => new Vector3(v.x, 0.0f, v.y);
}
