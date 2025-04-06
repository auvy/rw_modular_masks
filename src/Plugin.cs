using System.Reflection;
using MonoMod.RuntimeDetour;

using BepInEx;
using MoreSlugcats;
using RWCustom;
using System.Security.Permissions;
using UnityEngine;
using System;
using System.Collections.Generic;

using System.Runtime.Remoting.Contexts;
using MonoMod.Cil;

using UnityEngine.Networking;


using System.Collections;
using System.IO;


using System.Runtime.CompilerServices;
using BepInEx.Logging;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.Remoting.Messaging;
using UnityEngine.Experimental.Rendering;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Drawing;


using Color = UnityEngine.Color;
using Graphics = UnityEngine.Graphics;
using Vector2 = UnityEngine.Vector2;
using Rewired.UI.ControlMapper;
using System.Drawing.Drawing2D;




// Allows access to private members
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618


namespace ModularMasks;

using static Colors;
using static Constants;
using static Hashing;
using static Globals;
using static Naming;
using static Textures;
using static Baking;
using static Files;
using static CustomMask;
using static PreLoading;

using static HooksMask;
using static HooksVulture;
using static HooksScav;


[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
public class ModularMasksMain : BaseUnityPlugin
{
    public const string PLUGIN_GUID = "modularmasks";
    public const string PLUGIN_NAME = "Modular Vulture Masks";
    public const string PLUGIN_VERSION = "0.0.10.23";

    public static ManualLogSource PLogger { get; internal set; } = null;

    /// <summary>
    /// (Hook) updates the dictionary of pieces by counting them in loaded external mod spritesheets upon loading game.
    /// </summary>
    private void Hook_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);

        LoadPieceSettings(SETTINGS_FILE);

        LoadAtlases();
    }




    BindingFlags propFlags = BindingFlags.Instance | BindingFlags.Public;
    BindingFlags myMethodFlags = BindingFlags.Static | BindingFlags.Public;
    private void OnEnable()
    {
        PLogger = Logger;


        On.RainWorld.OnModsInit += Hook_OnModsInit;


        On.Vulture.DropMask += Hook_Vulture_DropMask;
        On.Vulture.InitiateGraphicsModule += Hook_Vulture_InitiateGraphicsModule;
        On.VultureGraphics.DrawSprites += Hook_VultureGraphics_DrawSprites;


        On.Scavenger.Violence += Hook_Scavenger_Violence;
        On.Scavenger.InitiateGraphicsModule += Hook_Scavenger_InitiateGraphicsModule;



        On.VultureMask.ctor += Hook_VultureMask_Constructor;
        On.AbstractPhysicalObject.Destroy += Hook_AbstractPhysicalObject_Destroy;


        //On.MoreSlugcats.VultureMaskGraphics.
        _ = new Hook(typeof(VultureMaskGraphics).GetProperty(nameof(VultureMaskGraphics.BaseTotalSprites)).GetGetMethod(), Hook_VultureMaskGraphics_BaseTotalSprites_Getter);

        On.MoreSlugcats.VultureMaskGraphics.ctor_PhysicalObject_MaskType_int_string += Hook_VulturaMaskGraphics_Constructor;

        On.MoreSlugcats.VultureMaskGraphics.InitiateSprites += Hook_VultureMaskGraphics_InitiateSprites;
        On.MoreSlugcats.VultureMaskGraphics.DrawSprites += Hook_VultureMaskGraphics_DrawSprites;
        On.MoreSlugcats.VultureMaskGraphics.AddToContainer += Hook_VultureMaskGraphics_AddToContainer;

        On.MoreSlugcats.VultureMaskGraphics.stringOffsets += Hook_stringOffsets;
    }
}
