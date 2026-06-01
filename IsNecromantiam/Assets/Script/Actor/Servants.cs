using Unity.VisualScripting;
using UnityEngine;

public class Servants : Actor
{
    [SerializeField] StateMachine m_StateMachine = null;

    public Transform m_Target = null;

    protected override void Reset()
    {
        m_StateMachine = this.GetOrAddComponent<StateMachine>();
        base.Reset();
    }

    protected override void Awake()
    {
        base.Awake();

        var machine = m_StateMachine;
        {
            machine.Add("Idle", new ActorStateIdle(this, 1.0f, "Chase"));
            //state.Add("Walk", new ActorStateRandomRoteWalk(this, 10.0f, "Idle"));
            var state = machine.Add("Chase", new ActorStateWalkTarget(this, m_Target, 1.0f, "Idle", "Idle"));
        }
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

    public void SetTarget(Transform target)
    {
        m_Target = target;
    }
}
