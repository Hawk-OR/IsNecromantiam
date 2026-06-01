using System;
using System.Collections.Generic;
using UnityEngine;

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

public class ActorStateScript
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

    public IReadOnlyList<Action> StartFunc => m_StartFunc;
    public IReadOnlyList<Action> ExitFunc => m_ExitFunc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        if (m_StartFunc != null) foreach (var i in m_StartFunc) i();
    }

    // Update is called once per frame
    public virtual void Update()
    {

    }

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

    public ActorStateScript AddStartFuncs(Action[] func)
    {
        if (m_StartFunc == null) m_StartFunc = new List<Action>();
        m_StartFunc.AddRange(func);
        return this;
    }

    public void ClearStartFunc()
    {
        m_StartFunc = null;
    }

    public ActorStateScript AddExitFunc(Action func)
    {
        if (m_ExitFunc == null) m_ExitFunc = new List<Action>();
        m_ExitFunc.Add(func);
        return this;
    }

    public ActorStateScript AddExitFuncs(Action[] func)
    {
        if (m_ExitFunc == null) m_ExitFunc = new List<Action>();
        m_ExitFunc.AddRange(func);
        return this;
    }

    public void ClearExitFunc()
    {
        m_ExitFunc = null;
    }

    protected bool CanLookPoint(Vector3 point)
    {
        var dir = point - m_Actor.transform.position;
        var length = dir.magnitude;
        if (length > m_Actor.Parameters.EyeSight) return false;

        var angle = Vector3.Angle(m_Actor.transform.forward, dir);
        if (MathF.Abs(angle) > m_Actor.Parameters.ViewAngle) return false;

        Vector3
            start = m_Actor.transform.position + Vector3.up * 0.5f,
            end = point + Vector3.up * 0.5f;
        return !Physics.Linecast(start, end, LayerMask.GetMask("Ground"));
    }
}
