using UnityEngine;

public class ActorStateMoveTarget : ActorStateScript
{
    private float m_Distance = 1.0f;

    bool m_IsRun = false;

    public ActorStateMoveTarget(Actor actor, float distance, string _true, string _false, bool run = false) : base(actor)
    {
        m_Distance = distance;
        m_IsRun = run;

        m_Transition = new(new[] {
            new Transition(_false, CheckTargetNull),
            new Transition(_false,()=>!CanLookPoint(m_Actor.Target)),
            new Transition(_true, CheckDistance),
        });
    }

    public ActorStateMoveTarget(Actor actor, Transition[] transitions) : base(actor, transitions)
    {

    }

    public override void Update()
    {
        if (target == null) return;

        var vec = target.position - transform.position;
        vec.y = 0.0f;

        m_Actor.AddRotate(vec.normalized);
        m_Actor.AddWalk(m_Actor.transform.forward, m_IsRun);
    }

    public bool CheckTargetNull() => target == null;

    private bool CheckDistance()
    {
        if (target == null) return false;
        var distance = Vector3.Distance(m_Actor.transform.position, target.position);
        return distance < m_Distance;
    }
}
