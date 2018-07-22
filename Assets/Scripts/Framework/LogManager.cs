using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogManager {

    public static bool IsOpen = true;

    public static void Log(object message)
    {
        Log(message, null);
    }

    public static void Log(object message, Object context)
    {
        if (IsOpen)
        {
            Debug.Log(message, context);
        }
    }

    public static void LogError(object message)
    {
        LogError(message, null);
    }

    public static void LogError(object message, Object context)
    {
        if (IsOpen)
        {
            Debug.LogError(message, context);
        }
    }

    public static void LogWarning(object message)
    {
        LogWarning(message, null);
    }

    public static void LogWarning(object message, Object context)
    {
        if (IsOpen)
        {
            Debug.LogWarning(message, context);
        }
    }


    public static void LogException(System.Exception exception)
    {
        if (IsOpen)
        {
            Debug.LogException(exception);
        }
    }

    public static void LogFormat(string format, params object[] args)
    {
        if (IsOpen)
        {
            Debug.LogFormat(format, args);
        }
    }
}
