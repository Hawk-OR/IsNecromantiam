using Unity.VisualScripting;
using UnityEngine;

public class Servants : Actor
{
    [SerializeField] StateMachine m_StateMachine = null;

    protected override void Reset()
    {
        m_StateMachine = this.GetOrAddComponent<StateMachine>();
        base.Reset();
    }

    protected override void Awake()
    {
        base.Awake();
        m_StateMachine.Add("Idle", new ActorStateIdle(this, "Walk", 1.0f));
        m_StateMachine.Add("Walk", new ActorStateRandomRoteWalk(this, "Idle", 5.0f));
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    protected override void Update()
    {

        base.Update();
    }
}
