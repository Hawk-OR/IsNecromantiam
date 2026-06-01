using UnityEngine;

public class ActorStateIdle : ActorStateScript
{
    private float m_IdleTime = 0.0f;
    private float m_Timer = 0.0f;

    public ActorStateIdle(Actor actor, string next, float transitionTime) : this(actor, new Transition[] { })
    {
        m_IdleTime = transitionTime;

        m_Transition.Clear();
        m_Transition.Add(new Transition(next, () => m_Timer <= 0.0f));
    }

    public ActorStateIdle(Actor actor, Transition[] transition) : base(actor, transition)
    {
    }

    public override void Start()
    {
        base.Start();
        m_Timer = m_IdleTime;
    }

    public override void Update()
    {
        m_Timer -= Time.deltaTime;
    }
}
