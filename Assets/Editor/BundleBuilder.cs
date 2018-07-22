using UnityEngine;
using UnityEditor;
using System.Collections;

public class BundleBuilder
{

    [MenuItem("Assets/Build AssetBundle From Selection")]
    public static void BuildResource()
    {
        string path = EditorUtility.SaveFilePanel("Save Resource", "", Selection.activeObject.name, "assets");
        if (path.Length > 0)
        {
            BuildAssetBundleOptions options =
                BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.UncompressedAssetBundle;
            BuildPipeline.BuildAssetBundle(Selection.activeObject, null, path, options, BuildTarget.Android);
        }
    }


    [MenuItem("Tools/Build AssetBundles")]
    public static void BuildBundles()
    {
        string path = Application.dataPath + "/../Bundles";
        BuildPipeline.BuildAssetBundles(path,
                                        BuildAssetBundleOptions.CollectDependencies
                                           | BuildAssetBundleOptions.UncompressedAssetBundle
                                        | BuildAssetBundleOptions.ForceRebuildAssetBundle
                                        ,
                                        EditorUserBuildSettings.activeBuildTarget);

    }

    [MenuItem("Tools/Check AssetBundles")]
    public static void CheckBundles()
    {


        string[] assetNames = AssetDatabase.GetSubFolders("Assets/Resources/Prefabs/UI");

        foreach (var assetName in assetNames)
        {
            Debug.Log(assetName);
        }

    }
}
