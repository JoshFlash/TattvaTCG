using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Log
{
    [Conditional("GAME_LOGGER")]
    public static void Info(object obj, string label = "[INFO] ", UnityEngine.LogType logType = UnityEngine.LogType.Log)
    {
        UnityEngine.Debug.unityLogger.Log(logType, label + obj);
    }
}