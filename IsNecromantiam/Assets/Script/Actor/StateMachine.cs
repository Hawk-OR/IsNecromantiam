using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class StateMachine : MonoBehaviour
{
    [SerializeField] Dictionary<string, ActorStateScript> m_Machine = null;
    [SerializeField] string m_CurrentState = "Idle";

    private void Awake()
    {
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!(m_Machine != null && ChangeState(m_CurrentState)))
            UnEnableMessage($"Failed to change state to {m_CurrentState}");
    }

    // Update is called once per frame
    void Update()
    {
        var current = GetCurrentState();

        current.Update();
        if (current.OnTransition(out var next))
            if (!ChangeState(next))
                UnEnableMessage($"Failed to change state to {next}");
    }

    public bool ChangeState(string name)
    {
        if (m_Machine.TryGetValue(name, out var next))
        {
            m_Machine[m_CurrentState].Exit();

            m_CurrentState = name;
            next.Start();
            return true;
        }
        return false;
    }

    private void UnEnableMessage(string name)
    {
        Debug.LogError(name);
        this.enabled = false;
    }

    public ActorStateScript Add(string name, ActorStateScript state)
    {
        if (m_Machine == null) m_Machine = new Dictionary<string, ActorStateScript>();

        m_Machine.Add(name, state);
        if (m_Machine.Count == 1) m_CurrentState = name;

        return state;
    }

    public ActorStateScript Get(string name) => m_Machine.TryGetValue(name, out var state) ? state : null;

    private ActorStateScript GetCurrentState() => m_Machine[m_CurrentState];

#if UNITY_EDITOR
    [CustomEditor(typeof(StateMachine))]
    private class StateMachineEditor : Editor
    {
        private StateMachine machine = null;

        private bool foldout = true;

        private List<bool> folders = new List<bool>();

        private void OnEnable()
        {
            machine = target as StateMachine;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(machine), typeof(StateMachine), false);

                EditorGUILayout.TextField("Current State", machine.m_CurrentState);
            }
            EditorGUI.EndDisabledGroup();

            if (machine.m_Machine == null) return;

            foldout = EditorGUILayout.Foldout(foldout, $"StateList ({machine.m_Machine.Count})");

            folders = new(machine.m_Machine.Count);
            if (foldout)
            {
                EditorGUI.indentLevel++;

                for (int i = 0; i < machine.m_Machine.Count; i++)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    {
                        EditorGUILayout.TextField($"State {i}", machine.m_Machine.ElementAt(i).Key);
                    }
                    EditorGUI.EndDisabledGroup();

                    //var transitions = machine.m_Machine.ElementAt(i).Value.Transition;

                    //EditorGUI.indentLevel++;

                    //if (folders.Count <= i) folders.Add(true);
                    //folders[i] = EditorGUILayout.Foldout(folders[i], $"Next States ({transitions.Count})");

                    //if (folders[i])
                    //{
                    //    EditorGUI.indentLevel++;
                    //    EditorGUI.BeginDisabledGroup(true);
                    //    {
                    //        foreach (var t in transitions)
                    //        {
                    //            EditorGUILayout.TextField($"Next State", t.Destination());
                    //        }
                    //    }
                    //    EditorGUI.EndDisabledGroup();

                    //    EditorGUI.indentLevel--;
                    //}

                    //EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }
        }
    }
#endif
}
