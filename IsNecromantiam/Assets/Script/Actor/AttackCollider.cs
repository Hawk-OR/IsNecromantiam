using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditorInternal;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class AttackCollider : MonoBehaviour
{
    private Actor m_Owner = null;

    [SerializeField] float m_Damage = 1.0f;
    [SerializeField] float m_KnockBack = 1.0f;

    enum Mode
    {
        Timer,
        Infinito,
    }

    [SerializeField] Mode m_Mode = Mode.Timer;
    [SerializeField] float m_Time = 1.0f;

    [SerializeField] List<Collider> m_Colliders = null;

    [SerializeField] List<Actor> m_BeHit = new();

    [SerializeField] bool m_Destroy = false;

    private string OwnerTag => m_Owner != null ? m_Owner.tag : "";

    public AttackCollider Attack(Actor actor, bool onTime = true)
    {
        m_Owner = actor;
        m_BeHit = new() { actor };

        gameObject.SetActive(true);

        if (onTime) StartCoroutine(Timer(m_Time));

        return this;
    }

    public void End() => Timer(0.0f);

    private IEnumerator Timer(float time)
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
        if (m_Destroy) Destroy(gameObject);
    }

    /// <summary>
    /// destory option
    /// </summary>
    /// <returns></returns>
    public AttackCollider Destoroy()
    {
        m_Destroy = true;
        return this;
    }

    private void Reset()
    {
        m_Colliders = new(this.GetComponentsInChildren<Collider>());
    }

    private void Awake()
    {
        m_Colliders = new(this.GetComponentsInChildren<Collider>());
        m_Destroy = false;

        gameObject.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var hits = new List<Actor>();
        foreach (var collider in m_Colliders)
        {
            var collide = HitCollider(collider);
            var actors = new List<Actor>();
            foreach (var actor in collide) actors.Add(actor.GetComponent<Actor>());
            hits.AddRange(hits.Concat(actors).Distinct());
        }

        hits = new(hits.Except(m_BeHit).Where(i => i != null));
        foreach (var actor in hits)
        {
            if (actor.tag != OwnerTag)
            {
                var knockBack = actor.transform.position - m_Owner.transform.position;
                actor.AddDamage(m_Damage, knockBack.normalized * m_KnockBack);
            }
        }

        m_BeHit.AddRange(hits);
    }

    private Collider[] HitCollider(Collider collider)
    {
        if (collider == null) return new Collider[0];
        var result = new List<Collider>();
        result.AddRange(HitBoxCollider(collider));
        if (result.Count < 1) result.AddRange(HitSphereCollider(collider));
        if (result.Count < 1) result.AddRange(HitCapsuleCollider(collider));

        return result.ToArray();
    }

    private Collider[] HitBoxCollider(Collider collider)
    {
        var box = collider.GetComponent<BoxCollider>();
        if (box == null) return new Collider[0];

        var center = transform.localToWorldMatrix.GetPosition() + box.center;
        var extent = Vector3.Scale(box.size * 0.5f, transform.lossyScale);

        return Physics.OverlapBox(center, extent, transform.rotation, LayerMask.GetMask("Actor"));
    }

    private Collider[] HitSphereCollider(Collider collider)
    {
        var sphere = collider.GetComponent<SphereCollider>();
        if (sphere == null) return new Collider[0];

        var center = transform.localToWorldMatrix.GetPosition() + sphere.center;
        var radius = transform.lossyScale.max() * sphere.radius;

        return Physics.OverlapSphere(center, radius, LayerMask.GetMask("Actor"));
    }

    private Collider[] HitCapsuleCollider(Collider collider)
    {
        var capsule = collider.GetComponent<CapsuleCollider>();
        if (capsule == null) return new Collider[0];

        var point1 = transform.localToWorldMatrix.GetPosition() + capsule.center;
        var point2 = point1 + (direction() * capsule.height);

        return Physics.OverlapCapsule(point1, point2, capsule.radius, LayerMask.GetMask("Actor"));

        Vector3 direction()
        {
            switch (capsule.direction)
            {
                case 0: return Vector3.right;
                case 1: return Vector3.up;
                case 2: return Vector3.forward;

                default: return Vector3.zero;
            }
        }
    }

    public bool IsDestory() => m_Destroy;

#if UNITY_EDITOR
    [CustomEditor(typeof(AttackCollider))]
    private class MyEditor : Editor
    {
        AttackCollider my = null;

        private void OnEnable()
        {
            my = (AttackCollider)target;
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();

            my.m_Owner = EditorGUILayout.ObjectField("Owner", my.m_Owner, typeof(Actor), true) as Actor;

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Damage", GUILayout.Width(64));
                my.m_Damage = EditorGUILayout.DelayedFloatField(my.m_Damage);

                EditorGUILayout.LabelField("KnockBack", GUILayout.Width(64));
                my.m_KnockBack = EditorGUILayout.DelayedFloatField(my.m_KnockBack);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            my.m_Mode = (Mode)EditorGUILayout.EnumPopup("Mode", my.m_Mode);

            EditorGUILayout.BeginHorizontal();
            {
                if (my.m_Mode == Mode.Timer)
                {
                    EditorGUILayout.LabelField("Timer", GUILayout.Width(64));
                    my.m_Time = EditorGUILayout.DelayedFloatField(my.m_Time);
                }

                EditorGUILayout.LabelField("Kill", GUILayout.Width(32));
                my.m_Destroy = EditorGUILayout.Toggle(my.m_Destroy);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
            {
                if (GUILayout.Button("Start"))
                {
                    my.Attack(null);
                }
            }
            EditorGUI.EndDisabledGroup();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(my, "Change Property");
                EditorUtility.SetDirty(my);
            }
        }
    }
#endif
}
