using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Actor
{
    [SerializeField] Camera m_Camera = null;
    private PlayerInput m_Input = null;

    [SerializeField] Transform m_Target = null;

    [Header("Summon Parameter")]
    [SerializeField] SummonParam m_SummonParam = new();
    [System.Serializable]
    private class SummonParam
    {
        public List<Object> summonPrefab = new();
        public float radius = 2f;
        public float speed = 3f;
        public uint amount = 1;
    }
    private float m_SummonTimer = 0.0f;

    [SerializeField] List<Servants> m_Servants = new();

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
        {
            if (rt > 0.0f)
            {
                Gamepad.current.SetMotorSpeeds(rt, rt);
                m_SummonTimer += Time.deltaTime;

                if (m_SummonTimer >= m_SummonParam.speed)
                {
                    m_SummonTimer = 0.0f;

                    var param = m_SummonParam;
                    for (int i = 0; i < param.amount; i++)
                    {
                        var length = Random.Range(0.0f, param.radius);
                        var vec = Vector3Extensions.AngleTo(Random.Range(0.0f, 360.0f));
                        var pos = m_Target.position + vec * length;
                        var spawnObj = param.summonPrefab[Random.Range(0, param.summonPrefab.Count)] as GameObject;
                        var obj = Instantiate<GameObject>(spawnObj, pos, transform.rotation);
                        obj.AddComponent<Servants>();
                        m_Servants.Add(obj.GetComponent<Servants>());
                    }
                }
            }
            else
            {
                Gamepad.current.SetMotorSpeeds(0.0f, 0.0f);
            }
        }

        for (int i = 0; i < m_Servants.Count;)
        {
            if (m_Servants[i] == null) m_Servants.RemoveAt(i);
            else i++;
        }

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

        Vector3 summonR = Vector3.one * m_SummonParam.radius * 2.0f;
        summonR.y = m_Target.lossyScale.y;
        m_Target.localScale = summonR;

        m_Animator.SetFloat("Move", m_Velocity.magnitude / m_Parameter.WalkSpeed);
    }

    private void OnSetControlMode(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<float>();

        if (value > 0.5f) m_ControlMode = ControlMode.Target;
        else m_ControlMode = ControlMode.Camera;
    }
}
