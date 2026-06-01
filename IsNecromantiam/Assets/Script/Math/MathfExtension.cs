using UnityEngine;

public static class MathfExtension
{
    public static Vector2 AngleToVector2(float angle)
    {
        var rad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
