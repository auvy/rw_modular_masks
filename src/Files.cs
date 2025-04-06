using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
﻿using BepInEx.Logging;

//using System.Text.Json;


using Newtonsoft.Json;


using RWCustom;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Playables;

namespace ModularMasks;

using static Constants;

public class Files
{
    public static string CACHE_PATH = getModRootPath() + CACHE_FOLDER;
    public static string SETTINGS_FILE = getModRootPath() + SETTINGS_FILE_NAME;


    public static string getModRootPath()
    {
        ModManager.Mod ok = ModManager.ActiveMods.Find(x => x.id == ModularMasksMain.PLUGIN_GUID);
        return ok.path + "/";
    }

    /// <summary>
    /// Outputs  a texture.
    /// </summary>
    public static void SaveTextureToPNG(Texture2D texture, string name)
    {
        try
        {
            PNGSaver.SaveTextureToFile(texture, CACHE_PATH + name + ".png");
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public static SettingsObject ReadJson(string path)
    {
        string jsonData = File.ReadAllText(path);

        return (SettingsObject)JsonConvert.DeserializeObject(jsonData, typeof(SettingsObject));
    }


    public static void OutputObjToJson(string path, object obj)
    {
        string json = JsonConvert.SerializeObject(obj, Formatting.Indented);

        File.WriteAllText(path, json);
    }



    public static bool FileExists(string path)
    {
        return File.Exists(path);
    }
}
