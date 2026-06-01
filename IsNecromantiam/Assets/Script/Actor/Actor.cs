using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [System.Serializable]
    public class Parameter
    {
        [Tooltip("The weight of the actor, affecting movement and gravity.")]public float Weight = 20.0f;

        [Header("Move")]
        public float WalkSpeed = 2.0f;
        public float Accelerator = 2.0f;
        public float Brake = 2.0f;
        public float RotationSpeed = 90.0f;

        [Header("Status")]
        public float HP = 100.0f;
        public float EyeSight = 10.0f;
        public float ViewAngle = 120.0f;
        public float Jump = 1.0f;
    }

    [Tooltip("Actor's parameters.")]
    [SerializeField, CustomLabel("Actor")] protected Parameter m_Parameter = new();

    protected bool m_IsGround = false;
    protected const float k_Gravity = 9.81f;
    protected static int? s_GroundLayer = null;
    protected const float k_UnderGround = -100.0f;

    protected float p_Resistance => 2.0f * (m_Parameter.Weight / 10.0f);

    protected Vector3 m_Velocity = Vector3.zero;

    protected Vector3 m_RotateVec = Vector3.zero;

    [SerializeField] protected Animator m_Animator = null;

    public Parameter Parameters => m_Parameter;

    protected virtual void Reset()
    {
        m_Animator = this.GetOrAddComponent<Animator>();
    }

    protected virtual void Awake()
    {
        Initialized();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //  move
        var pos = transform.position + m_Velocity * Time.deltaTime;
        if (pos.y < k_UnderGround) Destroy(gameObject);
        transform.position = pos;

        //  rotate
        if (m_RotateVec.magnitude > 0.0f)
        {
            var target = Quaternion.LookRotation(m_RotateVec);
            var rotate = Quaternion.RotateTowards(transform.rotation, target, m_Parameter.RotationSpeed * Time.deltaTime);
            transform.rotation = rotate;
        }

        //  resistance
        var horizontal = new Vector2(m_Velocity.x, m_Velocity.z);
        var length = Mathf.Max(horizontal.magnitude - p_Resistance * Time.deltaTime, 0.0f);
        var vec = horizontal.normalized * length;
        m_Velocity = new(vec.x, m_Velocity.y, vec.y);
    }

    protected virtual void FixedUpdate()
    {
        Gravity();
    }

    protected void Gravity()
    {
        var pos = transform.position; var prevGround = m_IsGround;
        if (Physics.Linecast(pos + Vector3.up, pos - Vector3.up * 0.25f, out var hit, s_GroundLayer.Value))
        {
            m_IsGround = true;
            if (prevGround != m_IsGround)
            {
                m_Velocity.y = 0;
                pos.y = hit.point.y;
                transform.position = pos;
            }
        }
        else
        {
            var v = m_Velocity.y - (m_Parameter.Weight / 10f) * k_Gravity * Time.fixedDeltaTime;
            var max = -2.0f * (m_Parameter.Weight / 10.0f) * k_Gravity;
            m_Velocity.y = Mathf.Max(v, max);
            m_IsGround = false;
        }
    }

    protected void Initialized()
    {
        m_IsGround = false;
        if (s_GroundLayer == null) s_GroundLayer = LayerMask.GetMask("Ground");
        m_Velocity = Vector3.zero;
        m_RotateVec = transform.forward;
    }

    /// <summary>
    /// Updates the current velocity by applying the specified walk vector, adjusting for acceleration or braking as
    /// appropriate.
    /// </summary>
    /// <remarks>This method modifies the velocity based on the input vector and current movement parameters.
    /// If the input vector has a magnitude greater than zero, acceleration is applied up to the configured walk speed.
    /// If the input vector is zero, braking is applied to reduce velocity. The Y component of the velocity is not
    /// affected.</remarks>
    /// <param name="value">walk vector representing the desired movement direction and magnitude. The method will adjust the velocity based on
    /// performed.</param>
    public void AddWalk(Vector3 value)
    {
        var vec = value;
        if (value.magnitude > 0.0f && Mathf.Abs(RotateAngle()) < 45.0f)
        {
            var length = (m_Velocity.magnitude > m_Parameter.WalkSpeed) ? 0.0f : m_Parameter.Accelerator;
            vec = value * (length * Time.deltaTime);
        }
        else
        {
            var length = m_Parameter.Brake * Time.deltaTime;
            vec = -(m_Velocity.xz().normalized * length).xz();
        }

        m_Velocity += vec;
    }

    public void AddRotate(Vector3 value) => m_RotateVec = value;

    public float RotateAngle() => Vector3.Angle(transform.forward, m_RotateVec);

    public void AddVelocity(Vector3 value) => m_Velocity += value;

    protected void AddJump(bool canJump = true)
    {
        if (canJump)
        {
            m_Velocity.y += m_Parameter.Jump * Time.deltaTime;
        }
    }

    protected bool IsNullChecks(object[] checks)
    {
        int result = 0;

        foreach (var check in checks) if (check == null) result++;

        if (result == 0) return false;

        Debug.LogError($"Null check failed: {result} null reference(s) found.");
        return true;
    }
}
