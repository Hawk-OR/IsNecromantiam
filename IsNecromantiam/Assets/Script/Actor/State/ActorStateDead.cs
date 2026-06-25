using UnityEngine;

public class ActorStateDead : ActorStateScript
{
    public ActorStateDead(Actor actor) : base(actor)
    {
    }
    public override void Start()
    {
        base.Start();
        Object.Destroy(m_Actor.gameObject);
    }
}
