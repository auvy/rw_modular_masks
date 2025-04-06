using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModularMasks;

using static Files;
using static Globals;

public class Debug
{
    internal static ManualLogSource Logger => ModularMasksMain.PLogger;

    public static void LogInfo(object data)
    {
        if (GLOBAL_SETTINGS.debugLog) Logger.LogInfo(data);
    }
    public static void LogDebug(object data)
    {
        if (GLOBAL_SETTINGS.debugLog) Logger.LogDebug(data);
    }
    public static void LogMessage(object data)
    {
        if (GLOBAL_SETTINGS.debugLog) Logger.LogMessage(data);
    }
    public static void LogWarning(object data)
    {
        Logger.LogWarning(data);
    }
    public static void LogError(object data)
    {
        Logger.LogError(data);
    }
    public static void LogFatal(object data)
    {
        Logger.LogFatal(data);
    }

    public static void SaveToPNG(Texture2D texture, string name)
    {
        if (GLOBAL_SETTINGS.debugOutputImage) SaveTextureToPNG(texture, name);
    }



}
