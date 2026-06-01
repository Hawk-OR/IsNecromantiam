using UnityEngine;

public static class FloatExtensions
{
    // 角度からベクトルを生成
    public static Vector3 AngleToVector3(this float angle, float y = 0.0f) => new Vector3(Mathf.Cos(angle), y, Mathf.Sin(angle));

    public static float normalized(this float value) => value / Mathf.Abs(value);
}
