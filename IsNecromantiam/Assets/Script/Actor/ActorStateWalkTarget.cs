using UnityEngine;

public class ActorStateWalkTarget : ActorStateScript
{
    private Transform m_Target = null;
    private float m_Distance = 1.0f;

    public ActorStateWalkTarget(Actor actor, Transform target, float distance, string _true, string _false) : base(actor)
    {
        m_Target = target;
        m_Distance = distance;

        m_Transition = new(new[] {
            new Transition(_false, CheckTargetNull),
            new Transition(_false,()=>!CanLookPoint(m_Target.position)),
            new Transition(_true, CheckDistance),
        });
    }

    public ActorStateWalkTarget(Actor actor, Transition[] transitions) : base(actor, transitions)
    {

    }

    public override void Update()
    {
        var vec = m_Target.position - m_Actor.transform.position;
        vec.y = 0.0f;

        m_Actor.AddRotate(vec.normalized);
        m_Actor.AddWalk(m_Actor.transform.forward);
    }

    public bool CheckTargetNull() => m_Target == null;

    private bool CheckDistance()
    {
        if (m_Target == null) return false;
        var distance = Vector3.Distance(m_Actor.transform.position, m_Target.position);
        return distance < m_Distance;
    }

    public Transform GetTarget() => m_Target;
    public void SetTarget(Transform target) => m_Target = target;
}
