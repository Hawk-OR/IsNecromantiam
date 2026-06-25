using System.Collections.Generic;
using UnityEngine;

public static class ListExtension
{
    public static List<T> Shuffle<T>(this List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int o = Random.Range(0, list.Count);
            var t = list[i]; list[i] = list[o]; list[o] = t;
        }

        return list;
    }
}
