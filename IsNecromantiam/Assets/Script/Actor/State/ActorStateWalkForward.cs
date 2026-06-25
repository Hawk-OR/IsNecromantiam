using UnityEngine;

/// <summary>
/// Represents the walking forward state for an actor, enabling forward movement behavior within the actor's state
/// machine.
/// </summary>
/// <remarks>Use this state to transition an actor into a forward walking motion. This class is typically used as
/// part of an actor's state management system to control movement direction. Inherits from ActorStateScript, allowing
/// integration with custom transitions and actor logic.</remarks>
public class ActorStateWalkForward : ActorStateScript
{
    private float m_WalkTime = 1.0f;
    private float m_Timer = 0.0f;

    public ActorStateWalkForward(Actor actor, string next, float walkTime) : this(actor, new Transition[] { })
    {
        m_WalkTime = walkTime;

        m_Transition.Clear();
        m_Transition.Add(new Transition(next, () => m_Timer <= 0.0f));
    }

    public ActorStateWalkForward(Actor actor, Transition[] transitions) : base(actor, transitions)
    {
    }

    public override void Start()
    {
        base.Start();
        m_Timer = m_WalkTime;
    }

    public override void Update()
    {
        m_Actor.AddWalk(m_Actor.transform.forward);

        m_Timer -= Time.deltaTime;
    }
}
