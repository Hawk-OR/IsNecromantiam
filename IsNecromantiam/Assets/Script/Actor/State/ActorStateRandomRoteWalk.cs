using UnityEngine;

public class ActorStateRandomRoteWalk : ActorStateScript
{
    private float m_WalkTime = 1.0f;
    private float m_Timer = 0.0f;

    public ActorStateRandomRoteWalk(Actor actor, float walkTime, string next) : this(actor, new Transition[] { })
    {
        m_WalkTime = walkTime;

        m_Transition.Clear();
        m_Transition.Add(new Transition(next, () => m_Timer <= 0.0f));
    }

    public ActorStateRandomRoteWalk(Actor actor, Transition[] transitions) : base(actor, transitions)
    {
    }

    public override void Start()
    {
        base.Start();
        m_Timer = m_WalkTime;

        m_Actor.AddRotate(Random.Range(0.0f, 360.0f).AngleToVector3());
    }

    public override void Update()
    {
        m_Actor.AddWalk(m_Actor.transform.forward);

        m_Timer -= Time.deltaTime;
    }
}
