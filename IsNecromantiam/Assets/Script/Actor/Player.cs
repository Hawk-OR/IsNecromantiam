using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Actor
{
    [SerializeField] Camera m_Camera = null;
    private PlayerInput m_Input = null;

    [SerializeField] Transform m_Target = null;

    enum ControlMode
    {
        Camera,
        Target,
    }

    private ControlMode m_ControlMode = ControlMode.Camera;

    protected override void Reset()
    {
        base.Reset();
        m_Camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    protected override void Awake()
    {
        base.Awake();

        m_ControlMode = ControlMode.Camera;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Input = GameManager.GetInputActions();
        {
            m_Input.actions.FindAction("LTrigger").performed += OnSetControlMode;
        }

        if (IsNullChecks(new object[] { m_Input, m_Camera, m_Target }))
            gameObject.SetActive(false);
    }

    // Update is called once per frame
    protected override void Update()
    {
        var rt = m_Input.actions.FindAction("RTrigger").ReadValue<float>();
        if (Gamepad.current != null)
            if (rt > 0.0f)
                Gamepad.current.SetMotorSpeeds(rt, rt);
            else
                Gamepad.current.SetMotorSpeeds(0.0f, 0.0f);

        var targetPos = m_Target.position - transform.position;
        if (m_ControlMode == ControlMode.Target)
        {
            var axis = m_Input.actions.FindAction("Look").ReadValue<Vector2>();
            targetPos.z += axis.y * 5.0f * Time.deltaTime;
            targetPos.x += axis.x * 5.0f * Time.deltaTime;
        }

        var moveVec = Vector3.zero;
        {
            var axis = m_Input.actions.FindAction("Move").ReadValue<Vector2>();
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

        m_Target.position = transform.position + targetPos;
        m_Animator.SetFloat("Move", m_Velocity.magnitude / m_Parameter.WalkSpeed);
    }

    private void OnSetControlMode(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<float>();

        if (value > 0.5f) m_ControlMode = ControlMode.Target;
        else m_ControlMode = ControlMode.Camera;
    }
}
