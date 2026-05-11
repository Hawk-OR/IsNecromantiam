#if UNITY_EDITOR
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Build
{
    private static BuildSetting m_Setting = null;

    private static void Initialized()
    {
        m_Setting = Resources.Load<BuildSetting>("BuildSetting");
    }

    [MenuItem("Build/Test")]
    public static void Test()
    {
        Initialized();

        var version = m_Setting.GetVersionAndSetTimeStamp();
        Debug.Log($"version: {version}");
    }

    [MenuItem("Build/Build for Release")]
    public static void ReleaseBuild()
    {
        //  get version name
        string version = m_Setting.GetVersionAndSetTimeStamp();

        //  set copy file
        BuildEvent.s_CopyFilesPath = m_Setting.CopyFilesPath;

        //  create "exe" directory
        string directory = $"{Path.Combine(Application.dataPath, "../../../", "Exe")}/{version}";
        System.IO.Directory.CreateDirectory(directory);

        //  get build options
        string[] scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
        var activeTarget = EditorUserBuildSettings.activeBuildTarget;
        var activeGroup = BuildPipeline.GetBuildTargetGroup(activeTarget);

        //  get build options
        var build = new BuildPlayerOptions();
        build.scenes = scenes;
        build.locationPathName = $"{directory}/{PlayerSettings.productName}.exe";
        build.target = activeTarget;
        build.targetGroup = activeGroup;

        PlayerSettings.bundleVersion = version;

        //  build!!
        var report = BuildPipeline.BuildPlayer(build);

        if (report != null)
        {
            Debug.Log($"Build [{version}] in \"{PlayerSettings.productName}\"");

            //  ZIP!!!
            if (File.Exists($"{directory}.zip")) Debug.LogError($"not zip in {version}.zip");
            else ZipFile.CreateFromDirectory(directory, $"{directory}.zip");

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

    [MenuItem("Build/BuildfFor Debug")]
    public static void DebugBuild()
    {
        //  get "ReleaseNote.txt"
        string path = Application.dataPath + "/MyFolder/ReleaseNote.txt";

        //  get version name
        string[] texts = File.ReadAllLines(path);
        int i = 0; for (i = 0; i < texts[0].Length; ++i) if (texts[0][i] == '(') break;
        string version = texts[0].Substring(0, i);

        int j = 0; for (j = 0; j < version.Length; ++j) if (version[j] == 'a') break;
        if (j == version.Length)
        {
            version += "a01";
        }
        else
        {
            var alpha = version.Substring(j + 1, version.Length - (j + 1));
            var num = (int.Parse("01") + 1).ToString("00");
            version = $"{version.Substring(0, j)}a{num}";
        }

        //  write time stamp in "ReleaseNote.txt"
        texts[0] = $"{version}({System.DateTime.Now})";
        File.WriteAllLines(path, texts);

        //  set copy file
        BuildEvent.s_CopyFilesPath = m_Setting.CopyFilesPath;

        //  create "exe" directory
        string directory = $"{Path.Combine(Application.dataPath, "../../../", "Exe")}/{version}";
        System.IO.Directory.CreateDirectory(directory);

        //  get build options
        string[] scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
        var activeTarget = EditorUserBuildSettings.activeBuildTarget;
        var activeGroup = BuildPipeline.GetBuildTargetGroup(activeTarget);

        //  set debug

        //  get build options
        var build = new BuildPlayerOptions();
        build.scenes = scenes;
        build.locationPathName = $"{directory}/{PlayerSettings.productName}.exe";
        build.target = activeTarget;
        build.targetGroup = activeGroup;

        //  set Develop
        build.options = BuildOptions.Development;

        PlayerSettings.bundleVersion = version;

        //  build!!
        BuildPipeline.BuildPlayer(build);
        Debug.Log($"Build [{version}] in \"{PlayerSettings.productName}\"");

        //  Open file
        System.Diagnostics.Process.Start(directory);
    }
}
#endif
