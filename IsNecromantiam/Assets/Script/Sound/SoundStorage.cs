using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

[CreateAssetMenu(fileName = "SoundStorage", menuName = "Scriptable Objects/SoundStorage")]
public class SoundStorage : ScriptableObject
{
    [SerializeField] AudioMixerGroup m_MixerGroup = null;

    [SerializeField] List<Clips> m_Clips = new List<Clips>();

    [System.Serializable]
    private class Clips
    {
        public string name;
        public AudioClip clip;

        public Clips()
        {
            name = "";
            clip = null;
        }

        public Clips(string name, AudioClip clip)
        {
            this.name = name;
            this.clip = clip;
        }
    }

    public List<AudioClip> GetClips()
    {
        List<AudioClip> clips = new List<AudioClip>();
        foreach (var clip in m_Clips) clips.Add(clip.clip);
        return clips;
    }

    public AudioClip Get(string name)
    {
        return m_Clips.Find(c => c.name == name)?.clip;
    }

    public List<AudioClip> FindClips(Func<string, bool> predicate)
    {
        List<AudioClip> tmp = new(8);
        foreach (var c in m_Clips)
        {
            if (predicate(c.name))
            {
                tmp.Add(c.clip);
            }
        }
        return tmp;
    }

    public AudioClip this[string name] => m_Clips.Find(c => c.name == name)?.clip;

    public AudioMixer GetMixer() { return m_MixerGroup.audioMixer; }

    public AudioMixerGroup GetMixerGroup() { return m_MixerGroup; }

    // エディター拡張機能
#if UNITY_EDITOR
    [CustomEditor(typeof(SoundStorage))]
    private class MyEditor : Editor
    {
        private ReorderableList m_Reorderable = null;

        SoundStorage m_Storage = null;

        private void OnEnable()
        {
            m_Storage = target as SoundStorage;

            m_Reorderable = new ReorderableList(m_Storage.m_Clips, typeof(Clips), true, true, true, true);

            m_Reorderable.drawElementCallback += Draw;
            m_Reorderable.drawHeaderCallback += Header;
            m_Reorderable.elementHeightCallback += Height;
        }

        private void Draw(Rect rect, int index, bool isActive, bool isFocused)
        {
            var clip = m_Storage.m_Clips[index];

            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 48;

            EditorGUILayout.BeginHorizontal();
            {
                var prevW = rect.xMax; rect.xMax /= 2.0f;
                clip.name = EditorGUI.DelayedTextField(rect, "Name", clip.name);
                rect.xMin = rect.xMax; rect.xMax = prevW;
                clip.clip = EditorGUI.ObjectField(rect, "Clip", clip.clip, typeof(AudioClip), false) as AudioClip;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = prevLabelWidth;
        }

        private float Height(int index) => 20.0f;

        private void Header(Rect rect)
        {
            var prevW = rect.xMax; rect.xMax /= 2.0f;
            EditorGUI.LabelField(rect, "Clips");

            rect.xMin = rect.xMax; rect.xMax = prevW;
            var clips_ = m_Storage.m_Clips;
            int iC = Mathf.Max(EditorGUI.DelayedIntField(rect, "Size", clips_.Count), 0);
            if (iC > clips_.Count)
            {
                for (int i = 0, addCount = iC - clips_.Count; i < addCount; ++i)
                {
                    clips_.Add(new Clips());
                }
            }
            else if (iC < clips_.Count)
            {
                clips_.RemoveRange(iC, clips_.Count - iC);
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            m_Storage.m_MixerGroup = EditorGUILayout.ObjectField("MixerGroup", m_Storage.m_MixerGroup, typeof(AudioMixerGroup), true) as AudioMixerGroup;

            m_Reorderable.DoLayoutList();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_Storage, "Change Property");
                EditorUtility.SetDirty(m_Storage);
            }
        }
    }
#endif
}
