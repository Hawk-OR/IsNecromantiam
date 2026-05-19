using UnityEngine;

public static class Vector3Extensions
{
    public static Vector2 xz(this Vector3 v) => new Vector2(v.x, v.z);
}
