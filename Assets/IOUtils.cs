using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class IOUtils
{
    private IOUtils()
    {
    }

    //向文件中写数据
    public static bool WriteToFile(byte[] bytes, string path)
    {
        bool result = false;
        FileStream file = null;
        try
        {
            file = new FileStream(path, FileMode.Create);
            file.Write(bytes, 0, bytes.Length);
            file.Flush();
            result = true;
        }
        catch (IOException e)
        {
            Debug.LogException(e);
        }
        finally
        {
            if (file != null)
            {
                file.Close();
            }
        }

        return result;
    }

    //目录不存在，生成目录
    public static bool CreateDirectories(string path)
    {
        bool result = true;

        if (!Directory.Exists(path))
        {
            var dir = Directory.CreateDirectory(path);
            result = dir != null && dir.Exists;
        }

        return result;
    }

    //删除目录
    public static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    public const string ANDROID_ASSET_CLASS = "com.0373fang.util.IOUtils";
    private static AndroidJavaClass AndroidAssetManager = null;
    private static IntPtr methodID = IntPtr.Zero;
    private static AndroidJavaClass GetAndroidAssetManager()
    {
        if (AndroidAssetManager == null)
        {
            AndroidAssetManager = new AndroidJavaClass(ANDROID_ASSET_CLASS);
        }
        return AndroidAssetManager;
    }


    public static byte[] ReadFromAssetFile(string path)
    {
        AndroidJavaClass jc = GetAndroidAssetManager();
        object[] args = { path };
        if (methodID == null || methodID == IntPtr.Zero)
        {
            methodID = AndroidJNIHelper.GetMethodID<byte[]>(jc.GetRawClass(), "readFromAsset", args, true);
        }
        jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
        try
        {
            IntPtr array2 = AndroidJNI.CallStaticObjectMethod(jc.GetRawClass(), methodID, array);
            byte[] result = AndroidJNIHelper.ConvertFromJNIArray<byte[]>(array2);
            AndroidJNI.DeleteLocalRef(array2);
            return result;
        }
        finally
        {
            AndroidJNIHelper.DeleteJNIArgArray(args, array);
        }
    }

    public static byte[] ReadFromFile(string path)
    {
        if (path.StartsWith("/"))
        {
            if (File.Exists(path))
            {
                return (File.ReadAllBytes(path));
            }
            return null;
        }

        string fullpath = ResourceManager.DOWNLOAD_PATH + path;
        if (File.Exists(fullpath))
        {
            return (File.ReadAllBytes(fullpath));
        }

        fullpath = Application.streamingAssetsPath + "/" + path;

        if (Application.platform == RuntimePlatform.Android)
        {
            return ReadFromAssetFile(path);
        }
        else if (File.Exists(fullpath))
        {
            return (File.ReadAllBytes(fullpath));
        }

        return null;
    }

    // 判断文件是否存在
    public static bool IsFileExists(string path)
    {
        if (path.StartsWith("/"))
        {
            return File.Exists(path);
        }

        string fullpath = ResourceManager.DOWNLOAD_PATH + path;
        if (File.Exists(fullpath))
        {
            return true;
        }

        if (Application.platform == RuntimePlatform.Android || Application.isMobilePlatform)
        {
            AndroidJavaClass jc = GetAndroidAssetManager();
            return jc.CallStatic<bool>("isExist", path);
        }

        return false;
    }


    private const float MB_UNIT = 1024 * 1024.0f;
    private const float KB_UNIT = 1024.0f;

    public static string GetUnitFormat(long bytes)
    {
        if (bytes > MB_UNIT)
        {
            return string.Format("{0:F2}MB", bytes / MB_UNIT);
        }

        if (bytes > KB_UNIT)
        {
            return string.Format("{0:F2}KB", bytes / KB_UNIT);
        }

        return string.Format("{0}B", bytes);
    }
}

