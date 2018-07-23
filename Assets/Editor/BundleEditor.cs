using UnityEngine;
using UnityEditor;
using System.Collections;

using UnityEditorInternal;
using UnityEngineInternal;

using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using UnityEngine.Rendering;
using System;

public class BundleEditor
{
    [MenuItem("Bundle/Build Editor Bundles")]
    public static void BuildEditorBundles()
    {
        BundlePlatformBundles(EditorUserBuildSettings.activeBuildTarget, Application.persistentDataPath + "/download");
    }

    [MenuItem("Bundle/Build Android Bundles")]
    public static void BuildAndroidBundles()
    {
        BundlePlatformBundles(BuildTarget.Android, "Bundles/android");
    }

    [MenuItem("Bundle/Build iOS Bundles")]
    public static void BuildIOSBundles()
    {
        BundlePlatformBundles(BuildTarget.iOS, "Bundles/ios");
    }

    [MenuItem("Bundle/Build Windows Bundles")]
    public static void BuildWindowBundles()
    {
        BundlePlatformBundles(BuildTarget.StandaloneWindows, "Bundles/windows");
    }

    public static void BundlePlatformBundles(BuildTarget platform, string path)
    {
        string defaultManifestFile = path + "/" + ResourceManager.GetFileName(path);
        string manifestFile = path + "/Bundles";
        IOUtils.CreateDirectories(path);
        var manifest = BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression, platform);
        //var manifest = BuildPipeline.BuildAssetBundles (path, BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.UncompressedAssetBundle, platform);     
        if (File.Exists(manifestFile))
        {
            File.Delete(manifestFile);
        }
        File.Move(defaultManifestFile, manifestFile);
        Debug.LogFormat("Build Bundle  >> {0} Success", path);
    }

    [MenuItem("Bundle/Generate Bundle Names")]
    public static void GenerateAssetBundles()
    {
        if (EditorUtility.DisplayDialog("Warning", "Generate Bundle Names ?", "No", "Yes"))
        {
            return;
        }


        GenerateAssetBundles("Assets/Resources/UI", ".prefab", true);

        GenerateAssetBundle("Assets/Resources/WidgetConfig.txt", "WidgetConfig");

        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Bundle/Clear Bundle Names")]
    public static void ClearAssetBundles()
    {
        if (EditorUtility.DisplayDialog("Warning", "Clear Bundle Names ?", "No", "Yes"))
        {
            return;
        }
        ClearAssetBundles("Assets/Resources/UI", ".prefab");

        ClearAssetBundles("Assets/Resources/WidgetConfig.txt", "WidgetConfig");

        EditorUtility.ClearProgressBar();
    }


    private static void GenerateAssetBundles(string path, string filter = "", bool useFileName = false)
    {
        Debug.LogFormat("Build Texture Bundle {0}", path);

        string assetName = GenerateAssetBundleName(path);

        var files = GetBundleFiles(path, filter);

        EditorUtility.DisplayProgressBar(assetName, path, 0.0f / files.Length);

        int counter = 0;

        foreach (var file in files)
        {
            if (useFileName)
            {
                var filename = GenerateAssetBundleName(file.Substring(0, file.LastIndexOf(".")));
                assetName = GenerateAssetBundleName(filename);
            }
            GenerateAssetBundle(file, assetName);
            EditorUtility.DisplayProgressBar("Build Asset Bundle (" + assetName + ")", file, (float)++counter / files.Length);
        }


        var dirs = AssetDatabase.GetSubFolders(path);
        foreach (var dir in dirs)
        {
            GenerateAssetBundles(dir, filter, useFileName);
        }

    }

    private static void ClearAssetBundles(string path, string filter = "")
    {
        if (Directory.Exists(path) == false)
        {
            return;
        }
        Debug.LogFormat("Clear Bundle {0}", path);

        var files = GetBundleFiles(path, filter);

        EditorUtility.DisplayProgressBar("Clear Bundle ", path, 0.0f / files.Length);

        int counter = 0;

        foreach (var file in files)
        {
            GenerateAssetBundle(file, "", "");
            EditorUtility.DisplayProgressBar("Clear Bundle " + path, file, ++counter / files.Length);
        }

        var dirs = AssetDatabase.GetSubFolders(path);
        foreach (var dir in dirs)
        {
            ClearAssetBundles(dir, filter);
        }

    }


    private static void GenerateAssetBundle(string filepath, string assetName, string variantName = "assets")
    {

        var importer = AssetImporter.GetAtPath(filepath);
        if (!assetName.Equals(importer.assetBundleName))
        {
            importer.assetBundleName = assetName;
            if (!string.IsNullOrEmpty(assetName))
            {
                importer.assetBundleVariant = variantName;
            }
            importer.SaveAndReimport();
        }
    }

    private static string GenerateAssetBundleName(string path)
    {
        return path.Replace("Assets/", "").Replace("Resources/", "").ToLower();
    }

    private static string[] GetBundleFiles(string path, string filter)
    {
        List<string> results = new List<string>();
        var files = Directory.GetFiles(path, "*" + filter + ".meta", SearchOption.TopDirectoryOnly);

        foreach (var file in files)
        {
            var filepath = file.Replace(".meta", "");
            var fileinfo = new FileInfo(filepath);

            if (fileinfo.Attributes != FileAttributes.Directory && fileinfo.Attributes != FileAttributes.Hidden && !filepath.EndsWith(".cs") && !filepath.EndsWith(".mat") && !filepath.EndsWith(".fnt"))
            {
                results.Add(filepath);
            }
        }

        return results.ToArray();
    }


    public static void CopyFileOrDirectory(string source, string target, string filter)
    {

        if (File.Exists(target))
        {
            FileUtil.DeleteFileOrDirectory(target);
        }

        IOUtils.CreateDirectories(target);

        var files = Directory.GetFiles(source);
        foreach (var file in files)
        {
            if (file.EndsWith(filter))
            {
                continue;
            }
            var path = Path.Combine(target, Path.GetFileName(file));
            if (File.Exists(path))
            {
                FileUtil.ReplaceFile(file, path);
            }
            else
            {
                FileUtil.CopyFileOrDirectory(file, path);
            }
        }

        var dirs = Directory.GetDirectories(source);
        foreach (var dir in dirs)
        {
            var path = Path.Combine(target, Path.GetFileName(dir));
            CopyFileOrDirectory(dir, path, filter);
        }
    }
}
