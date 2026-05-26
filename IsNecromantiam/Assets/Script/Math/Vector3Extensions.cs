using UnityEngine;

public static class Vector3Extensions
{
    public static Vector2 xz(this Vector3 v) => new Vector2(v.x, v.z);

    public static Vector3 AngleTo(float angle, float y = 0.0f) => new Vector3(Mathf.Cos(angle), y, Mathf.Sin(angle));
}
