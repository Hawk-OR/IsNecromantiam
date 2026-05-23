using Unity.VisualScripting;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [System.Serializable]
    protected class Parameter
    {
        public float Weight = 20.0f;
        [Header("Move")]
        public float WalkSpeed = 2.0f;
        public float Accelerator = 2.0f;
        public float Brake = 2.0f;
        public float RotationSpeed = 5.0f;

        [Space(10)]
        public float Jump = 1.0f;
    }

    [SerializeField, CustomLabel("Actor")] protected Parameter m_Parameter = new();

    protected bool m_IsGround = false;
    protected const float k_Gravity = 9.81f;
    protected static int? s_GroundLayer = null;

    protected float p_Resistance => 2.0f * (m_Parameter.Weight / 10.0f);

    protected Vector3 m_Velocity = Vector3.zero;

    [SerializeField] protected Animator m_Animator = null;

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
        transform.position = pos;

        //  
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
    }

    protected void AddMove(Vector3 value)
    {
        var vec = value;
        if (value.magnitude > 0.0f)
        {
            var length = (m_Velocity.xz().magnitude > m_Parameter.WalkSpeed) ? 0.0f : m_Parameter.Accelerator;
            vec = value * (length * Time.deltaTime);
        }
        else
        {
            var length = -Mathf.Max(m_Velocity.magnitude - m_Parameter.Brake * Time.deltaTime, 0.0f);
            vec = m_Velocity.xz().normalized * length;
        }

        vec.y = 0.0f; m_Velocity += vec;
    }

    protected void AddJump(bool canJump = true)
    {
        if (canJump)
        {
            m_Velocity.y += m_Parameter.Jump * Time.deltaTime;
        }
    }
}
