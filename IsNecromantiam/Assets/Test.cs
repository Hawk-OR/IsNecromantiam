using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    private void Awake()
    {
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<int>
            a = new(new[] { 1, 2, 3 }),
            b = new(new[] { 3, 4, 5 });

        List<int> result = new(a.Except(b));
        Debug.Log(result.Count);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDebug(InputAction.CallbackContext context)
    {
        Debug.Log("Debug");
    }
}
