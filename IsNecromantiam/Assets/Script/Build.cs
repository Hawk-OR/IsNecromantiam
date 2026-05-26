#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.UIElements;
using UnityEngine;

public class Build : EditorWindow
{
    [SerializeField] private static string m_CompanyName = "";

    private static BuildSetting m_Setting = null;

    private static bool Initialized()
    {
        return m_Setting != null;
    }

    [MenuItem("Build/Test")]
    public static void Test()
    {
        Initialized();

        var version = m_Setting.GetVersionAndSetTimeStamp();
        Debug.Log($"version: {version}");
    }

    private void Reset()
    {
        m_CompanyName = PlayerSettings.companyName;
    }

    private static void ReleaseBuild()
    {
        if (!Initialized()) { Debug.LogError("Please set Build Setting!!"); return; }

        //  get version name
        string version = m_Setting.GetVersionAndSetTimeStamp();

        //  set copy file
        BuildEvent.s_CopyFilesPath = m_Setting.CopyFilesPath;

        //  create "exe" directory
        string directory = CreateExeDirectory(version);

        Debug.Log(directory);

        //  get build options
        var build = SetBuildOptions(version, directory);

        //  build!!
        var report = BuildPipeline.BuildPlayer(build);

        CreateZipFile(report, version, directory);
    }

    public static void DebugBuild()
    {
        if (!Initialized()) { Debug.LogError("Please set Build Setting!!"); return; }

        //  get version name
        string version = m_Setting.GetVersion();

        int j = 0; for (j = 0; j < version.Length; ++j) if (version[j] == 'a') break;
        if (j == version.Length)
        {
            version += "a01";
        }
        else
        {
            var alpha = version.Substring(j + 1, version.Length - (j + 1));
            var num = (int.Parse(alpha) + 1).ToString("00");
            version = $"{version.Substring(0, j)}a{num}";
        }

        //  write time stamp in "ReleaseNote.txt"
        m_Setting.WriteLine($"ver{version}({System.DateTime.Now})");

        //  set copy file
        BuildEvent.s_CopyFilesPath = m_Setting.CopyFilesPath;

        //  create "exe" directory
        string directory = CreateExeDirectory(version);

        //  get build options
        var build = SetBuildOptions(version, directory);

        //  set Develop
        build.options = BuildOptions.Development;

        //  build!!
        var report = BuildPipeline.BuildPlayer(build);

        CreateZipFile(report, version, directory);
    }

    private static string CreateExeDirectory(string version)
    {
        string directory = m_Setting.ExePath;
        Debug.Log(directory);
        System.IO.Directory.CreateDirectory(directory);
        return directory;
    }

    private static BuildPlayerOptions SetBuildOptions(string version, string directory)
    {
        PlayerSettings.bundleVersion = version;
        PlayerSettings.productName = m_Setting.ProductName;
        PlayerSettings.companyName = m_CompanyName;

        PlayerSettings.fullScreenMode = m_Setting.GetScreenMode(out var size);
        PlayerSettings.defaultScreenWidth = size.x;
        PlayerSettings.defaultScreenHeight = size.y;

        string[] scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
        var activeTarget = EditorUserBuildSettings.activeBuildTarget;
        var activeGroup = BuildPipeline.GetBuildTargetGroup(activeTarget);

        //  get build options
        var build = new BuildPlayerOptions();
        build.scenes = scenes;
        build.locationPathName = $"{directory}/{m_Setting.ExeName}.exe";
        build.target = activeTarget;
        build.targetGroup = activeGroup;

        return build;
    }

    private static void CreateZipFile(BuildReport report, string version, string directory)
    {
        if (report != null)
        {
            Debug.Log($"Build [{version}] in \"{m_Setting.ProductName}\"");

            //  ZIP!!!
            if (File.Exists($"{directory}.zip")) File.Delete($"{directory}.zip");
            ZipFile.CreateFromDirectory(directory, $"{directory}.zip");

            //  Open file
            System.Diagnostics.Process.Start(directory);

            Debug.Log("End Build GG");
        }
        else
        {
            Directory.Delete(directory, true);

            Debug.Log("Build Error!!!");
        }
    }

    [MenuItem("Build/Open Build Window")]
    private static void Open()
    {
        var window = GetWindow<Build>("Build");
        window.Show();
    }

    private void OnGUI()
    {
        m_CompanyName = EditorGUILayout.TextField("Company Name", m_CompanyName);

        EditorGUILayout.Space();

        m_Setting = EditorGUILayout.ObjectField("Build Setting", m_Setting, typeof(BuildSetting), false) as BuildSetting;

        if (GUILayout.Button("Build for Release")) ReleaseBuild();

        EditorGUI.BeginChangeCheck();

        EditorGUI.EndChangeCheck();
    }
}
#endif
