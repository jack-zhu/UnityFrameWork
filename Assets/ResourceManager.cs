using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager {

    //下载路径存储路径
    public static readonly string DOWNLOAD_PATH = Application.persistentDataPath + "/download";

    //ab资源缓存池
    private static Dictionary<string, AssetBundleInfo> bundleCache = new Dictionary<string, AssetBundleInfo>();

    //resouce资源缓存池
    private static Dictionary<string, GameObject> resourceCache = new Dictionary<string, GameObject>();

    //ab 的manifestinfo
    private static AssetBundleManifest assetBundleManifest = null;

    ResourceManager()
    {
        
    }

    //资源加载init阶段，先读取ab中的manifest文件，根据manifest再读取assetbundle和关联的ab
    public static bool Init()
    {
        string path = "Bundles";
        byte[] bytes = IOUtils.ReadFromFile(path);
        if (bytes != null && bytes.Length > 0)
        {
            var assetBundle = AssetBundle.LoadFromMemory(bytes);
            assetBundleManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundlesManifest");
        }else
        {
            Debug.LogFormat("load manifest {0} failed",path);
        }
        return assetBundleManifest != null;
    }

    //load assetbundl
    private static AssetBundle LoadAssetBundle(string path, bool cascade = true)
    {
        AssetBundleInfo assetBundleInfo = null;
        string abName = path;
        //先从budlecache里面查看是否已经缓存，没有则新读取并加入缓存，有则从缓存直接读取
        if (!bundleCache.TryGetValue(path,out assetBundleInfo))
        {
            var bytes = IOUtils.ReadFromFile(path);
            if (bytes != null && bytes.Length > 0)
            {
                if (cascade)
                {
                    LoadDependencies(abName);
                }
                AssetBundle assetBundle = AssetBundle.LoadFromMemory(bytes);
                assetBundleInfo = new AssetBundleInfo(assetBundle);
                bundleCache.Add(path, assetBundleInfo);
            }

        }
        return assetBundleInfo.assetBundle;
    }

    //加载关联的ab
    private static void LoadDependencies(string abName)
    {
        if (assetBundleManifest == null)
        {
            Init();
        }
        var abNames = assetBundleManifest.GetAllDependencies(abName);
        foreach (var item in abNames)
        {
            var assetBundle = LoadAssetBundle(item, false);
            if (assetBundle == null)
            {
#if UNITY_EDITOR
                Debug.LogFormat("load dependences {0}", item);
#endif
            }
        }
    }

    private static void DestroyAssetBundle(string abName, bool cascade = true)
    {
        AssetBundleInfo assetBundleInfo = null;
        if (bundleCache.TryGetValue(abName, out assetBundleInfo))
        {
#if UNITY_EDITOR
            Debug.LogFormat("DestroyAssetBundle {0}", abName);
#endif
            bundleCache.Remove(abName);
            assetBundleInfo.assetBundle.Unload(true);

            //删除关联的ab
            if (cascade)
            {
                string[] abArray = assetBundleManifest.GetDirectDependencies(abName);
                foreach (var item in abArray)
                {
                    DestroyAssetBundle(item, false);
                }
            }
        }

    }
}
