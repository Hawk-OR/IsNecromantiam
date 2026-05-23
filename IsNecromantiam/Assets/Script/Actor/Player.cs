using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Actor
{
    [SerializeField] Camera m_Camera = null;
    private InputActionMap m_Input = null;

    protected override void Reset()
    {
        base.Reset();
        m_Camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Input = GetComponent<PlayerInput>()?.currentActionMap;
        if (m_Input == null || m_Camera == null) gameObject.SetActive(false);
    }

    // Update is called once per frame
    protected override void Update()
    {
        var axis = m_Input.FindAction("Move").ReadValue<Vector2>();
        var moveVec = Vector3.zero;
        {
            var forward = m_Camera.transform.forward; forward.y = 0.0f;
            var right = m_Camera.transform.right; right.y = 0.0f;
            moveVec = forward.normalized * axis.y + right.normalized * axis.x;
            moveVec.Normalize();
        }
        AddMove(moveVec.normalized);

        if (moveVec.magnitude > 0.0f)
        {
            var look = Quaternion.LookRotation(moveVec, transform.up);
            var roto = Quaternion.SlerpUnclamped(transform.rotation, look, m_Parameter.RotationSpeed * Time.deltaTime);
            transform.rotation = roto;
        }

        base.Update();

        m_Animator.SetFloat("Move", m_Velocity.magnitude / m_Parameter.WalkSpeed);
    }
}
