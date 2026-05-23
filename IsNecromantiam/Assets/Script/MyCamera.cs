using UnityEngine;
using UnityEngine.InputSystem;

public class MyCamera : MonoBehaviour
{
    [SerializeField] Transform m_Target = null;

    [SerializeField] Vector3 m_Offset = Vector3.zero;

    [SerializeField] Vector3 m_LookPoint = new(0.0f, 1.0f, 0.0f);
    [SerializeField] float m_CameraLength = 2.0f;

    public Vector2 m_CameraAngle = Vector2.zero;

    [SerializeField] float m_MoveSpeed = 1.0f;
    [SerializeField] float m_RotaSpeed = 90.0f;

    InputActionMap m_Input = null;

    private void Awake()
    {
        m_Input = InputSystem.actions.FindActionMap("Player");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_Target == null) this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        var lookPoint = m_Target.position + m_LookPoint;

        var move = m_Target.position;
        {
            var offset = Quaternion.LookRotation(m_Offset).eulerAngles;

            var axis = m_Input.FindAction("Look").ReadValue<Vector2>();
            var speed = m_RotaSpeed * Time.deltaTime;

            m_CameraAngle.x = m_CameraAngle.x + axis.x * speed;
            m_CameraAngle.y = Mathf.Clamp(m_CameraAngle.y + axis.y * speed, -80f, +80f);

            var angleXZ = m_CameraAngle.x + offset.y * Mathf.Deg2Rad;
            Quaternion xz = Quaternion.AngleAxis(angleXZ, Vector3.up);

            var angleXY = m_CameraAngle.y + offset.x * Mathf.Deg2Rad;
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
}
