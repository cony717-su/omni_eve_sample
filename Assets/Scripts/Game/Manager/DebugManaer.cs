using UnityEngine;
using System;
using System.Collections.Generic;

public static class DebugManager
{
    static string Path
    {
        get
        {
            string trace = StackTraceUtility.ExtractStackTrace();
            trace = trace.Substring(trace.IndexOf("\n") + 1);
            trace = trace.Substring(trace.IndexOf("\n") + 1);
            trace = trace.Substring(trace.IndexOf("\n") + 1);
            return trace.Substring(0, trace.IndexOf("\n") + 1);    
        }
    }
    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Log (object message = null)
    {
        if (message is null)
        {
            Debug.Log(Path);
            return;
        }
        Debug.Log (message);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Log (object message, UnityEngine.Object context)
    {   
        Debug.Log (message, context);
    }
		 
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogWarning (object message = null)
    {   
        if (message is null)
        {
            Debug.Log(Path);
            return;
        }
        Debug.LogWarning (message.ToString ());
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")] 
    public static void LogWarning (object message, UnityEngine.Object context)
    {   
        Debug.LogWarning (message.ToString (), context);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogError (object message = null)
    {   
        if (message is null)
        {
            Debug.Log(Path);
            return;
        }
        Debug.LogError (message);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]	
    public static void LogError (object message, UnityEngine.Object context)
    {   
        Debug.LogError (message, context);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Assert(bool condition)
    {
        if (!condition) throw new Exception();
    }
}