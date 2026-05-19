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
        GameManager.Instance().ChangeScene("PlayScene", 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
