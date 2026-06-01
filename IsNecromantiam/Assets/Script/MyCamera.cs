using UnityEngine;
using UnityEngine.InputSystem;

public class MyCamera : MonoBehaviour
{
    [SerializeField] Transform m_Target = null;

    [SerializeField] Vector3 m_Offset = Vector3.zero;

    [SerializeField] Vector3 m_LookPoint = new(0.0f, 1.0f, 0.0f);
    [SerializeField] float m_CameraLength = 2.0f;

    private Vector2 m_CameraAngle = Vector2.zero;

    [SerializeField] float m_MoveSpeed = 1.0f;
    [SerializeField] float m_RotaSpeed = 90.0f;

    PlayerInput m_Input = null;

    private bool m_IsControl = false;

    private void Awake()
    {
        var offset = Quaternion.LookRotation(m_Offset).eulerAngles;
        m_CameraAngle.x += offset.y; m_CameraAngle.y += offset.x;

        m_IsControl = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_Target == null) this.enabled = false;

        m_Input = GameManager.GetInputActions();
        {
            m_Input.actions.FindAction("LTrigger").performed += OnSetControl;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Target == null) this.enabled = false;

        var lookPoint = m_Target.position + m_LookPoint;

        var move = m_Target.position;
        {
            var axis = m_IsControl ? m_Input.actions.FindAction("Look").ReadValue<Vector2>() : Vector2.zero;
            var speed = m_RotaSpeed * Time.deltaTime;

            m_CameraAngle.x = m_CameraAngle.x + axis.x * speed;
            m_CameraAngle.y = Mathf.Clamp(m_CameraAngle.y + axis.y * speed, -80f, +80f);

            var angleXZ = m_CameraAngle.x;
            Quaternion xz = Quaternion.AngleAxis(angleXZ, Vector3.up);

            var angleXY = m_CameraAngle.y;
            Quaternion yz = Quaternion.AngleAxis(angleXY, transform.right);

            Vector3 cameraVec = (yz * xz) * Vector3.forward;
            move = lookPoint + (cameraVec * m_CameraLength);
        }
        var pos = Vector3.LerpUnclamped(transform.position, move, m_MoveSpeed * Time.deltaTime);
        transform.position = pos;

        var look = Quaternion.LookRotation(lookPoint - transform.position, transform.up);
        var rote = Quaternion.SlerpUnclamped(transform.rotation, look, m_MoveSpeed * Time.deltaTime);
        var angle = rote.eulerAngles; angle.z = 0.0f;
        transform.rotation = Quaternion.Euler(angle);
    }

    private void OnSetControl(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<float>();

        if (value > 0.5f) m_IsControl = false;
        else m_IsControl = true;
    }
}
