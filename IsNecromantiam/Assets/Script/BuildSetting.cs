
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

    private enum ExeNameMode
    {
        Manual,
        ProductName,
        Version,
        ProductNameAndVersion,
        VersionAndProductName,
    }

    [SerializeField] ExeNameMode m_ExeName = ExeNameMode.ProductNameAndVersion;
    [SerializeField] string m_Exe = "";

    [SerializeField] ExeNameMode m_ExeFileName = ExeNameMode.ProductNameAndVersion;
    [SerializeField] string m_ExeFile = "";

    [SerializeField] string m_ExePath = "../../../Exe";

    [SerializeField] List<Object> m_CopyFiles = new List<Object>();

    [SerializeField] FullScreenMode m_ScreenMode = FullScreenMode.FullScreenWindow;
    [SerializeField] Vector2Int m_ScreenSize = new(1920, 1080);

    public string ProductName => m_ProductName;

    private void Reset()
    {
        m_ProductName = PlayerSettings.productName;
    }

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

                var lines = new List<string>(text);
                lines.Insert(0, "ver0.0.1");
                File.WriteAllLines(ReleaseNotePath, lines);
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

    public void WriteLine(string text, int index = 0)
    {
        if (m_Mode != Mode.ReleaseNote || m_ReleaseNote == null) return;
        var lines = m_ReleaseNote.text.Split('\n');
        lines[index] = text;
        File.WriteAllLines(ReleaseNotePath, lines);
    }

    public string GetVersionAndSetTimeStamp()
    {
        string result = "0.0.1";
        switch (m_Mode)
        {
            case Mode.Manual: result = m_BundleVersion; break;
            case Mode.ReleaseNote:
                if (m_ReleaseNote == null) return "0.0.1";
                var text = new List<string>(File.ReadAllLines(ReleaseNotePath));
                int i = 0; for (i = 0; i < text[0].Length; ++i) if (text[0][i] == '(') break;
                var ver = text[0].Substring(0, i);
                if (i > 3 && ver.Substring(0, 3) == "ver") result = ver.Substring(3).Trim();

                text[0] = $"ver{result}({System.DateTime.Now})";
                File.WriteAllLines(ReleaseNotePath, text);
                break;
            default: result = "0.0.1"; break;
        }
        return result;
    }

    public string ReleaseNotePath => AssetDatabase.GetAssetPath(m_ReleaseNote);

    public string ExeName => m_Exe;

    public string ExeFileName => m_ExeFile;

    public string ExePath => $"{Application.dataPath}\\{m_ExePath}\\{m_ExeFile}".Trim();

    public List<Object> CopyFiles => m_CopyFiles;

    public List<string> CopyFilesPath
    {
        get
        {
            List<string> paths = new List<string>();

            foreach (var o in m_CopyFiles)
            {
                if (o == null) continue;
                var path = AssetDatabase.GetAssetPath(o);
                if (!string.IsNullOrEmpty(path)) paths.Add(path);
            }
            return paths;
        }
    }

    public FullScreenMode GetScreenMode() => m_ScreenMode;

    public FullScreenMode GetScreenMode(out Vector2Int screenSize)
    {
        screenSize = m_ScreenSize;
        return m_ScreenMode;
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

            if (GUILayout.Button("Reload")) ReLoad();

            m_Setting.m_ProductName = EditorGUILayout.TextField("Product Name", m_Setting.m_ProductName);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Screen Mode", GUILayout.Width(80));
                m_Setting.m_ScreenMode = (FullScreenMode)EditorGUILayout.EnumPopup(m_Setting.m_ScreenMode, GUILayout.Width(130));

                if (m_Setting.m_ScreenMode == FullScreenMode.Windowed)
                {
                    EditorGUILayout.LabelField("Screen Size", GUILayout.Width(70));
                    m_Setting.m_ScreenSize = EditorGUILayout.Vector2IntField("", m_Setting.m_ScreenSize);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            m_Setting.m_Mode = (Mode)EditorGUILayout.EnumPopup("Mode", m_Setting.m_Mode);

            switch (m_Setting.m_Mode)
            {
                case Mode.Manual: DrawManual(m_Setting); break;
                case Mode.ReleaseNote: DrawReleaseNote(m_Setting); break;
                default: break;
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Exe Name", GUILayout.Width(70));

                m_Setting.m_ExeName = (ExeNameMode)EditorGUILayout.EnumPopup(m_Setting.m_ExeName);

                switch (m_Setting.m_ExeName)
                {
                    case ExeNameMode.Manual:
                        m_Setting.m_Exe = EditorGUILayout.TextField(m_Setting.m_Exe);
                        break;
                    default:
                        EditorGUILayout.LabelField(m_Setting.m_Exe);
                        break;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Exe File", GUILayout.Width(70));

                m_Setting.m_ExeFileName = (ExeNameMode)EditorGUILayout.EnumPopup(m_Setting.m_ExeFileName);

                switch (m_Setting.m_ExeFileName)
                {
                    case ExeNameMode.Manual:
                        m_Setting.m_ExeFile = EditorGUILayout.TextField(m_Setting.m_ExeFile);
                        break;
                    default:
                        EditorGUILayout.LabelField(m_Setting.m_ExeFile);
                        break;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                var length = Application.dataPath.Length;
                EditorGUILayout.LabelField($".....{Application.dataPath.Substring(length - 28)}/");
                m_Setting.m_ExePath = EditorGUILayout.TextField(m_Setting.m_ExePath);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            m_List.DoLayoutList();

            if (EditorGUI.EndChangeCheck())
            {
                SetName(m_Setting.m_ExeName, ref m_Setting.m_Exe);
                SetName(m_Setting.m_ExeFileName, ref m_Setting.m_ExeFile);

                Undo.RecordObject(m_Setting, "Change BuildSetting");
                EditorUtility.SetDirty(m_Setting);
            }
        }

        private void SetName(ExeNameMode mode, ref string name)
        {
            switch (mode)
            {
                case ExeNameMode.ProductName: name = m_Setting.ProductName; break;
                case ExeNameMode.Version: name = $"ver{m_Setting.GetVersion()}"; break;
                case ExeNameMode.ProductNameAndVersion: name = $"{m_Setting.ProductName}_ver{m_Setting.GetVersion()}"; break;
                case ExeNameMode.VersionAndProductName: name = $"ver{m_Setting.GetVersion()}_{m_Setting.ProductName}"; break;
                default: break;
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

        private void ReLoad()
        {
            SetName(m_Setting.m_ExeName, ref m_Setting.m_Exe);
            SetName(m_Setting.m_ExeFileName, ref m_Setting.m_ExeFile);
        }
    }
}

[CustomPropertyDrawer(typeof(BuildSetting))]
public class BuildSettingDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label, true);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif
