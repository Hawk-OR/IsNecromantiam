using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class AttackCollierGenerator : MonoBehaviour
{
    [SerializeField] Actor m_Owner = null;

    [SerializeField] List<Attack> m_Attacks = new();

    [System.Serializable]
    private class Attack
    {
        public string name = "";
        public AttackCollider collider = null;

        public Attack()
        {
            name = "";
            collider = null;
        }

        public Attack(string name, AttackCollider collider)
        {
            this.name = name;
            this.collider = collider;
        }
    }

    public AttackCollider this[string nmae] => m_Attacks.Find(i => i.name == nmae).collider;

    public void OnAttack(string name)
    {
        var collide = this[name];
        if (collide == null) return;

        if (collide.IsDestory()) GameObject.Instantiate(collide).Attack(m_Owner);
        else collide.Attack(m_Owner);
    }

    public void EndAttack(string name) => this[name].End();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var attack in m_Attacks) if (attack.collider != null)
        {
            var obj = attack.collider.gameObject;

            //  一度「Awake」を呼ぶためにアクティブにする
            obj.SetActive(true);
            //  そのあと非表示
            obj.SetActive(false);

#if !DEBUG
            obj.GetComponent<Renderer>().enabled = false;
#endif
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AttackCollierGenerator))]
    private class MyEditor : Editor
    {
        AttackCollierGenerator my = null;

        ReorderableList list = null;

        private void OnEnable()
        {
            my = (AttackCollierGenerator)target;

            list = new(my.m_Attacks, typeof(Attack), true, true, true, true);
            {
                list.drawHeaderCallback += Header;
                list.drawElementCallback += Draw;
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(my), typeof(AttackCollierGenerator), true);
            }
            EditorGUI.EndDisabledGroup();

            my.m_Owner = EditorGUILayout.ObjectField("Owner", my.m_Owner, typeof(Actor), true) as Actor;

            list.DoLayoutList();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(my, "Change Property");
                EditorUtility.SetDirty(my);
            }
        }

        private void Header(Rect rect)
        {
            var pw = rect.xMax; rect.xMax /= 2.0f;
            EditorGUI.LabelField(rect, "Attack Colliders");

            rect.xMin = rect.xMax; rect.xMax = pw;

            var attack = my.m_Attacks;
            int ic = Mathf.Max(EditorGUI.DelayedIntField(rect, "Size", attack.Count), 0);

            if (ic > attack.Count)
                for (int i = 0, addCount = ic - attack.Count; i < addCount; i++) attack.Add(new Attack());
            else if (ic < attack.Count)
                attack.RemoveRange(ic, attack.Count - ic);
        }

        private void Draw(Rect rect, int index, bool isActive, bool isFocused)
        {
            var attack = my.m_Attacks[index];

            var plw = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 48;

            EditorGUILayout.BeginHorizontal();
            {
                var pw = rect.xMax; rect.xMax /= 2;
                attack.name = EditorGUI.DelayedTextField(rect, "Name", attack.name);

                rect.xMin = rect.xMax; rect.xMax = pw;
                attack.collider = EditorGUI.ObjectField(rect, "Attack", attack.collider, typeof(AttackCollider), true) as AttackCollider;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = plw;
        }
    }
#endif
}
