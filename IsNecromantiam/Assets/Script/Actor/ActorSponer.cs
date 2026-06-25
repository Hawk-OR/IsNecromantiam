using System.Collections.Generic;
using UnityEngine;

public class ActorSponer : MonoBehaviour
{
    float m_Timer = 0.0f;
    [SerializeField] float m_Time = 1.0f;

    enum Mode
    {
        Rote,
        Randam,
        RandamRote,
    }

    [SerializeField] Mode m_Mode = Mode.Rote;
    private int m_Index = -1;

    [SerializeField] List<Actor> m_Spawns = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_Spawns.Count == 0) this.enabled = false;

        m_Timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        m_Timer -= Time.deltaTime;

        if (m_Timer <= 0.0f)
        {
            var index = Random.Range(0, m_Spawns.Count);
            var pos = transform.position;
            var rot = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.up);
            GameObject.Instantiate(m_Spawns[index], pos, rot);

            m_Timer = m_Time;
        }
    }

    private void SetIndex()
    {
        switch (m_Mode)
        {
            case Mode.Rote:
                m_Index = m_Index++ % m_Spawns.Count; break;
            case Mode.Randam:
                m_Index = Random.Range(0, m_Spawns.Count); break;
            case Mode.RandamRote:
                m_Index++; if (m_Index >= m_Spawns.Count) Shuffle(); break;
        }
    }

    private void Shuffle()
    {
        m_Spawns.Shuffle();

        m_Index = 0;
    }
}
