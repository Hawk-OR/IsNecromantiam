using System.Collections.Generic;
using UnityEngine;

public class ActorStateFixedSearch : ActorStateScript
{
    private StateMachine m_StateMachine = null;
    private List<string> m_SearchedState = null;

    public ActorStateFixedSearch(Actor actor, string _true, string[] _searchedState) : this(actor, _true)
    {
        m_StateMachine = actor.GetComponent<StateMachine>();
        m_SearchedState = new(_searchedState);
        m_FixedTrans = new() { new(_true, CanLookPoint) };
    }

    public ActorStateFixedSearch(Actor actor, string _true) : base(actor)
    {
        m_FixedTrans = new();
        m_FixedTrans.Add(new(_true, () => CanLookPoint(actor.Target)));
    }

    public bool CanLookPoint()
    {
        if (m_StateMachine != null)
        {
            if (m_StateMachine.CheckState(m_SearchedState.ToArray()))
                return CanLookPoint(m_Actor.Target);
            else
                return false;
        }
        else
            return CanLookPoint(m_Actor.Target);
    }
}
