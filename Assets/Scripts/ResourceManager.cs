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

    public static T LoadResource<T>(string path, string name) where T : Object
    {
        T obj = null;

        AssetBundle assetBundle = LoadAssetBundle(path.ToLower() + ".asset", false);
        if (assetBundle != null)
        {
            obj = assetBundle.LoadAsset<T>(name);
        }

        if (obj == null)
        {
            obj = Resources.Load<T>(GetResourcePath(path, name));
        }

        return obj;
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

    //销毁assetbundle
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

    //卸载资源（先看是否是assetbundle，然后再销毁resource文件夹的文件）
    public static void DestroyResource(string path)
    {
        string name = path.ToLower() + ".asset";
        DestroyAssetBundle(name);

        GameObject obj = null;
        if (resourceCache.TryGetValue(path, out obj))
        {
            resourceCache.Remove(path);
            Resources.UnloadAsset(obj);
        }
    }

    //获取dir + "/" + name
    private static string GetResourcePath(string path, string name)
    {
        string dirName = path.EndsWith(name) ? GetDirectoryPath(path) : path;
        return string.IsNullOrEmpty(dirName) ? name : dirName + "/" + name;
    }

    //获取文件名称
    public static string GetFileName(string path)
    {
        int index = path.LastIndexOf("/");
        if (index > 0)
        {
            return path.Substring(index + 1);
        }
        return path;
    }

    //获取文件夹路径
    public static string GetDirectoryPath(string path){
        int index = path.LastIndexOf("/");
        if (index > 0)
        {
            return path.Substring(0, index);
        }
        return path;
    }

    public static T LoadResource<T>(string path) where T :Object
    {
        return LoadResource<T>(GetDirectoryPath(path), GetFileName(path));
    }

    public static GameObject LoadResourceUI(string path)
    {
        return LoadResource<GameObject>("UI" + path);
    }

    public static Sprite LoadResourceSprite(string path)
    {
        return LoadResource<Sprite>(GetDirectoryPath(path), GetFileName(path));
    }

    public static TextAsset LoadResourceTextAsset(string path)
    {
        return LoadResource<TextAsset>(GetDirectoryPath(path), GetFileName(path));
    }
}
