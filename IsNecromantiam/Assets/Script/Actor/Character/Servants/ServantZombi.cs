using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class ServantZombi : Servants
{
    [SerializeField] StateMachine m_StateMachine = null;

    private bool m_IsMove = false;
    private float m_MoveSpeed = 0.0f;
    private float m_NextMoveSpeed = 0.0f;

    protected override void Reset()
    {
        m_StateMachine = this.GetOrAddComponent<StateMachine>();

        base.Reset();
    }

    protected override void Awake()
    {
        if (m_StateMachine == null) m_StateMachine = this.GetOrAddComponent<StateMachine>();
        {
            var machine = m_StateMachine;

            machine.Add("WakeUp", new ActorStateScript(this, new Transition("Idle", () => machine.Trigger("WakeUp"))))
                .AddStartFunc(() => SetMove(0.0f)).AddStartFunc(() => m_Animator.Play("zombie stand up"));

            machine.Add("Idle", new ActorStateIdle(this, 1.0f, "Walk")).AddStartFunc(() => SetMove(0.0f));
            machine.Add("Walk", new ActorStateRandomRoteWalk(this, 4.0f, "Idle")).AddStartFunc(() => SetMove(1.0f));

            machine.Add("SERCH", new ActorStateFixedSearch(this, "Case", new[] { "Idle", "Walk" }));

            machine.Add("Case", new ActorStateMoveTarget(this, 1.0f, "Attack", "Idle")).AddStartFunc(() => SetMove(1.0f));

            machine.Add("Attack", new ActorStateScript(this, new Transition("Idle", () => machine.Trigger("Attack"))))
                .AddStartFuncs(new() { () => SetMove(0.0f), () => m_Animator.SetTrigger("Attack") });
        }

        base.Awake();
    }

    protected override void Start()
    {
        if (IsNullChecks(new[] { m_StateMachine })) this.enabled = false;

        base.Start();
    }

    protected override void Update()
    {
        m_Animator.SetFloat("Move", m_MoveSpeed);

        if (m_IsMove)
        {
            m_IsMove = false;
            var duration = Mathf.Abs(m_MoveSpeed - m_NextMoveSpeed);
            DOVirtual.Float(m_MoveSpeed, m_NextMoveSpeed, duration, value => m_MoveSpeed = value);
        }

        base.Update();
    }

    private void SetMove(float moveSpeed)
    {
        m_NextMoveSpeed = moveSpeed;
        m_IsMove = true;
    }
}
