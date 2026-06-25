using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public Transform Target => m_Target;
    [SerializeField] protected Transform m_Target = null;
    protected bool m_SearchTarget = false;

    [SerializeField] protected List<Tag> m_FindTags = new();

    protected Dictionary<string, bool> m_DebugFlag = new() { { "gravity", false } };

    [System.Serializable]
    public class Parameter
    {
        [Tooltip("The weight of the actor, affecting movement and gravity.")] public float Weight = 20.0f;

        [Header("Move")]
        public float WalkSpeed = 2.0f;
        public float RunSpeed = 4.0f;
        public float Accelerator = 2.0f;
        public float Brake = 2.0f;
        public float RotationSpeed = 90.0f;
        public float SlopeAngle = 30.0f;
        public float Jump = 1.0f;
        public float FallResistance = 3.0f;
        public float EyeSight = 10.0f;
        public float ViewAngle = 120.0f;

        [Header("Status")]
        public float HP = 100.0f;
        [Range(0.0f, 100.0f)] public float protect = 0.0f;
    }

    [Tooltip("Actor's parameters.")]
    [SerializeField, CustomLabel("Actor")] protected Parameter m_Parameter = new();

    protected bool m_IsGround = false;
    protected const float k_Gravity = 9.81f;
    protected static int? s_GroundLayer = null;
    protected const float k_UnderGround = -100.0f;
    protected float m_LastGroundHigth = 0.0f;

    protected float p_Resistance => 2.0f * (m_Parameter.Weight / 10.0f);

    protected Vector3 m_Velocity = Vector3.zero;

    protected Vector3 m_RotateVec = Vector3.zero;

    public Animator Animator => m_Animator;
    [SerializeField] protected Animator m_Animator = null;

    [SerializeField] protected AttackCollierGenerator m_Attacks = null;

    public Parameter Parameters => m_Parameter;

    protected virtual void Reset()
    {
        m_Attacks = this.GetOrAddComponent<AttackCollierGenerator>();
        m_Animator = this.GetOrAddComponent<Animator>();
    }

    protected virtual void Awake()
    {
        Initialized();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        transform.position = GetGroundPos();
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

        if (Target == null) StartCoroutine(SetTarget());

        if (m_Parameter.HP <= 0.0f) Dead();
    }

    protected void Gravity()
    {
        var pos = transform.position;

        Vector3
            start = pos + Vector3.up,
            end = pos - Vector3.up * 0.25f;

        if (m_DebugFlag["gravity"]) Debug.DrawLine(start, end);

        var flag = Physics.Raycast(start, Vector3.down, out var hit, 1000.0f, s_GroundLayer.Value);

        if (flag && hit.distance < 1.25f)
        {
            m_IsGround = true;
            m_Velocity.y = Mathf.Max(0.0f, m_Velocity.y);

            if (m_Parameter.SlopeAngle < Vector3.Angle(hit.normal, Vector3.up))
            {
                var direction = hit.normal; direction.y *= -1;
                m_Velocity += direction;
            }

            //AddRotate(hit.normal);

            pos.y = hit.point.y;
            transform.position = pos;

            var d = m_LastGroundHigth - m_Parameter.FallResistance;
            if (d > 0.0f) AddDamage(d, Vector3.zero);

            m_LastGroundHigth = 0.0f;
        }
        else
        {
            var v = m_Velocity.y - (m_Parameter.Weight / 10f) * k_Gravity * Time.fixedDeltaTime;
            var max = -2.0f * (m_Parameter.Weight / 10.0f) * k_Gravity;
            m_Velocity.y = Mathf.Max(v, max);
        }

        if (flag && m_LastGroundHigth < hit.distance) m_LastGroundHigth = hit.distance;
    }

    protected void Initialized()
    {
        m_IsGround = false;
        if (s_GroundLayer == null) s_GroundLayer = LayerMask.GetMask("Ground");
        Set<Animator>(ref m_Animator);

        m_Velocity = Vector3.zero;
        m_RotateVec = transform.forward;
    }

    protected T Set<T>(ref T value, bool isAdd = false) where T : Component
    {
        if (value == null)
        {
            if (isAdd) value = this.GetOrAddComponent<T>();
            else value = GetComponent<T>();
        }

        return value;
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
    public void AddWalk(Vector3 value, bool isRun = false)
    {
        var vec = value;
        if (value.magnitude > 0.0f && Mathf.Abs(RotateAngle()) < 45.0f)
        {
            var max = isRun ? m_Parameter.WalkSpeed : m_Parameter.RunSpeed;
            var length = (m_Velocity.magnitude > max) ? 0.0f : m_Parameter.Accelerator;
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

    public virtual void AddDamage(float baseDamage, Vector3 knockBack)
    {
        var p = this.m_Parameter;

        var damage = baseDamage * (1.0f - p.protect);
        p.HP -= damage;

        AddVelocity(knockBack);
    }

    protected virtual void Dead()
    {
        this.enabled = false;
        Destroy(gameObject, 1.0f);
    }

    protected IEnumerator SetTarget()
    {
        if (m_SearchTarget) yield break;

        m_SearchTarget = true;

        var collide = Physics.OverlapSphere(transform.position, m_Parameter.EyeSight, LayerMask.GetMask("Actor"));

        if (collide.Length > 0)
        {
            foreach (var t in collide)
            {
                if (!(m_FindTags.Count > 0 ? CheckTag(t.tag) : this.tag != t.tag)) continue;

                yield return null;

                if (t != null && t.transform != transform)
                {
                    m_Target = t.transform;
                    break;
                }
            }
        }

        m_SearchTarget = false;

        bool CheckTag(string tag)
        {
            foreach (var t in m_FindTags)
            {
                if (tag == t) return true;
            }

            return false;
        }
    }

    protected float GetGroundHight()
    {
        var start = transform.position; start.y = 100.0f;

        if (Physics.Raycast(start, Vector3.down, out var hit, 1000.0f, s_GroundLayer.Value)) return hit.point.y;
        else return 0.0f;
    }

    protected Vector3 GetGroundPos()
    {
        var start = transform.position; start.y = 100.0f;

        if (Physics.Raycast(start, Vector3.down, out var hit, 1000.0f, s_GroundLayer.Value)) return hit.point;
        else return Vector3.zero;
    }
}
