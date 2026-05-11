
#if UNITY_EDITOR
using UnityEngine;

using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEditorInternal;

[CreateAssetMenu(fileName = "BuildSetting", menuName = "Scriptable Objects/BuildSetting")]
public class BuildSetting : ScriptableObject
{
    [SerializeField]
    string m_ProductName = "";

    private enum Mode
    {
        Manual = 0,
        ReleaseNote = 1,
    }

    [SerializeField] Mode m_Mode = Mode.Manual;

    [SerializeField] string m_BundleVersion = "0.0.1";
    [SerializeField] TextAsset m_ReleaseNote = null;

    [SerializeField] List<Object> m_CopyFiles = new List<Object>();

    public string GetVersion()
    {
        switch (m_Mode)
        {
            case Mode.Manual: return m_BundleVersion;
            case Mode.ReleaseNote:
                if (m_ReleaseNote == null) return "0.0.1";
                var text = m_ReleaseNote.text.Split('\n');
                int i = 0; for (i = 0; i < text[0].Length; ++i) if (text[0][i] == '(') break;
                var ver = text[0].Substring(0, i);
                if (i > 3 && ver.Substring(0, 3) == "ver") return ver.Substring(3);
                return "0.0.1";
            default: return "0.0.1";
        }
    }

    public void SetTimeStamp()
    {
        if (m_Mode != Mode.ReleaseNote || m_ReleaseNote == null) return;

        var text = m_ReleaseNote.text.Split('\n');
        int i = 0; for (i = 0; i < text[0].Length; ++i) if (text[0][i] == '(') break;
        var ver = text[0].Substring(0, i);
        text[0] = $"{ver}({System.DateTime.Now})";
        File.WriteAllLines(ReleaseNotePath, text);
    }

    public string GetVersionAndSetTimeStamp()
    {
        string result = "0.0.1";
        switch (m_Mode)
        {
            case Mode.Manual: result = m_BundleVersion; break;
            case Mode.ReleaseNote:
                if (m_ReleaseNote == null) return "0.0.1";
                var text = m_ReleaseNote.text.Split('\n');
                int i = 0; for (i = 0; i < text[0].Length; ++i) if (text[0][i] == '(') break;
                var ver = text[0].Substring(0, i);
                if (i > 3 && ver.Substring(0, 3) == "ver") result = ver.Substring(3);

                text[0] = $"ver{result}({System.DateTime.Now})";
                File.WriteAllLines(ReleaseNotePath, text);
                break;
            default: result = "0.0.1"; break;
        }
        return result;
    }

    public string ReleaseNotePath => AssetDatabase.GetAssetPath(m_ReleaseNote);

    public List<Object> CopyFiles => m_CopyFiles;

    public List<string> CopyFilesPath
    {
        get
        {
            List<string> paths = new List<string>();

            paths.Add(ReleaseNotePath);

            foreach (var o in m_CopyFiles)
            {
                if (o == null) continue;
                var path = AssetDatabase.GetAssetPath(o);
                if (!string.IsNullOrEmpty(path)) paths.Add(path);
            }
            return paths;
        }
    }

    //  Custom Editor
    [CustomEditor(typeof(BuildSetting))]
    private class MyEditor : Editor
    {
        BuildSetting m_Setting = null;

        private ReorderableList m_List = null;

        private void OnEnable()
        {
            m_Setting = target as BuildSetting;

            m_List = new ReorderableList(m_Setting.m_CopyFiles, typeof(Object), true, true, true, true);
            m_List.drawElementCallback += DrawCopyFiles;
            m_List.drawHeaderCallback += Header;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            m_Setting.m_ProductName = EditorGUILayout.TextField("Product Name", m_Setting.m_ProductName);

            EditorGUILayout.Space();

            m_Setting.m_Mode = (Mode)EditorGUILayout.EnumPopup("Mode", m_Setting.m_Mode);

            switch (m_Setting.m_Mode)
            {
                case Mode.Manual: DrawManual(m_Setting); break;
                case Mode.ReleaseNote: DrawReleaseNote(m_Setting); break;
                default: break;
            }

            EditorGUILayout.Space();

            m_List.DoLayoutList();
            //DrawDefaultInspector();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_Setting, "Change BuildSetting");
                EditorUtility.SetDirty(m_Setting);
            }
        }

        private void DrawManual(BuildSetting setting)
        {
            setting.m_BundleVersion = EditorGUILayout.TextField("version", setting.m_BundleVersion);
        }

        private void DrawReleaseNote(BuildSetting setting)
        {
            setting.m_ReleaseNote = EditorGUILayout.ObjectField("Release Note", setting.m_ReleaseNote, typeof(TextAsset), false) as TextAsset;
        }

        private void Header(Rect rect)
        {
            var prevW = rect.xMax; rect.xMax /= 2.0f;
            EditorGUI.LabelField(rect, "Copy Files");

            rect.xMin = rect.xMax; rect.xMax = prevW;

            var list = m_Setting.m_CopyFiles;
            int i = Mathf.Max(EditorGUI.DelayedIntField(rect, "Size", list.Count), 0);
            if (i > list.Count) list.AddRange(new Object[i - list.Count]);
            else if (i < list.Count) list.RemoveRange(i, list.Count - i);
        }
        private void DrawCopyFiles(Rect rect, int index, bool isActive, bool isFocused)
        {
            m_Setting.m_CopyFiles[index] = EditorGUI.ObjectField(rect, m_Setting.m_CopyFiles[index], typeof(Object), false);
        }
    }
}
#endif
