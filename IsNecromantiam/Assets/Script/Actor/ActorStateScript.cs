using System;
using System.Collections.Generic;

public class Transition
{
    private Func<bool> m_Conditions = () => true;
    private string m_Destination = "Idle";

    public Transition() { }

    public Transition(string destination, Func<bool> conditions = null)
    {
        m_Destination = destination;
        if (conditions != null) m_Conditions = conditions;
    }

    public string Destination() => m_Destination;

    public bool Condition() => m_Conditions();
    public bool Condition(out string next)
    {
        if (m_Conditions())
        {
            next = m_Destination;
            return true;
        }
        else
        {
            next = null;
            return false;
        }
    }
}

public abstract class ActorStateScript
{
    protected Actor m_Actor = null;
    protected List<Transition> m_Transition = new List<Transition>();

    protected List<Action> m_StartFunc = null;
    protected List<Action> m_ExitFunc = null;

    public ActorStateScript(Actor actor, Transition transition) : this(actor, new Transition[] { transition })
    {
    }

    public ActorStateScript(Actor actor, Transition[] transition = null)
    {
        m_Actor = actor;
        if (transition != null) m_Transition = new(transition);
    }

    public IReadOnlyList<Transition> Transition => m_Transition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        if (m_StartFunc != null) foreach (var i in m_StartFunc) i();
    }

    // Update is called once per frame
    public abstract void Update();

    public virtual void Exit()
    {
        if (m_ExitFunc != null) foreach (var i in m_ExitFunc) i();
    }

    public bool OnTransition(out string next)
    {
        foreach (var i in m_Transition)
        {
            if (i.Condition(out next)) return true;
        }
        next = null;
        return false;
    }

    public ActorStateScript AddStartFunc(Action func)
    {
        if (m_StartFunc == null) m_StartFunc = new List<Action>();
        m_StartFunc.Add(func);
        return this;
    }

    public ActorStateScript AddExitFunc(Action func)
    {
        if (m_ExitFunc == null) m_ExitFunc = new List<Action>();
        m_ExitFunc.Add(func);
        return this;
    }
}
