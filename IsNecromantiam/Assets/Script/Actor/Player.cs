using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Actor
{
    private InputActionMap m_Input = null;

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Input = GetComponent<PlayerInput>()?.currentActionMap;
        if (m_Input == null) gameObject.SetActive(false);
    }

    // Update is called once per frame
    protected override void Update()
    {
        var axis = m_Input.FindAction("Move");
        AddMove(axis.ReadValue<Vector2>());

        base.Update();
    }
}
