#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildEvent : IPostprocessBuildWithReport
{
    //[SerializeField] TextAsset m_ReleaseNote = null;
    public static List<string> s_CopyFilesPath = new List<string>();

    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.options == BuildOptions.Development)
        {
            Debug.LogWarning("this is debug");
        }
        else
        {
            foreach (var p in s_CopyFilesPath) CopyFile(report, p);

            var path = Path.GetDirectoryName(report.summary.outputPath);

            DeleteFiles(Directory.GetDirectories(path, "*DoNotShip"));
        }
    }

    private void CopyFile(BuildReport report, string path)
    {
        string newPath = $"{Path.GetDirectoryName(report.summary.outputPath)}/{Path.GetFileName(path)}";

        File.Copy(path, newPath, true);
        Debug.Log("Copying file \n in" + path + "\n to" + newPath);
    }

    private void DeleteFile(BuildReport report, string pattern)
    {
        var path = Path.GetDirectoryName(report.summary.outputPath);

        foreach (var i in Directory.GetDirectories(path, pattern))
        {
            Debug.Log($"Delete for {i}");
            Directory.Delete(i, true);
        }
    }

    private void DeleteFiles(string[] paths)
    {
        foreach (var path in paths) DeleteFile(path);
    }

    private void DeleteFile(string path)
    {
        Debug.Log($"Delete for {path}");
        Directory.Delete(path, true);
    }
}
#endif
