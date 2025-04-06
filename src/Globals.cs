using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using System.Reflection;
using MonoMod.RuntimeDetour;

using BepInEx;
using MoreSlugcats;
using RWCustom;
using System.Security.Permissions;




namespace ModularMasks;

using static ModularMasksMain;
using static Naming;
using static CustomMask;



public class Globals
{
    public static SettingsObject GLOBAL_SETTINGS = new SettingsObject();

    public static int CURRENT_TEMP_PEARLSTRING_VARIATION = 0;


    /// <summary>
    /// Scav warPaint Colors.
    /// </summary>
    public static List<Color> SCAV_PAINT_COLORS = new List<Color> { };

    public static List<Color> SCAV_CHIEF_BONE_COLORS = new List<Color> { };


    /// <summary>
    /// All pieces from GLOBAL_SETTINGS.json
    /// </summary>
    public static Dictionary<string, Piece> ALL_PIECE_DATA = new Dictionary<string, Piece>();

    public static List<List<List<PearlStringAttachPoint>>> PEARL_STRING_POINTS = new List<List<List<PearlStringAttachPoint>>>();

    /// <summary>
    /// Vault of atlases of all pieces.
    /// </summary>
    //public static Dictionary<string, Texture2D> LOADED_ATLASES_TEXTURES = new Dictionary<string, Texture2D>() { };

    /// <summary>
    /// list of all already baked masks.
    /// </summary>
    public static List<string> BakedMasks = new List<string>();

    /// <summary>
    /// list of all already baked mask paint.
    /// </summary>
    public static List<string> BakedScavPaintMasks = new List<string>();





    // vultures dont normally store anything about masks since they are generic in vanilla,
    // the ids of masks are only generated after the mask is dropped.

    // soo the solution is to pre-make a mask object when the vulture spawns, tie it to vulture with CWT.
    // and when the mask object is created, a new CustomMaskData is automatically generated, and added to another CWT.

    /// <summary>
    /// Get MaskObj from Vulture.
    /// </summary>
    public static ConditionalWeakTable<Vulture, VultureMask> vulture_to_maskobj_CWT = new ConditionalWeakTable<Vulture, VultureMask>();

    /// <summary>
    /// Get MaskObj from Elite Scav.
    /// </summary>
    public static ConditionalWeakTable<Scavenger, VultureMask> elitescav_to_maskobj_CWT = new ConditionalWeakTable<Scavenger, VultureMask>();

    /// <summary>
    /// Get CustomMask from MaskGFX.
    /// </summary>
    public static ConditionalWeakTable<VultureMaskGraphics, CustomMaskData> maskgfx_to_custommask_CWT = new ConditionalWeakTable<VultureMaskGraphics, CustomMaskData>();

    /// <summary>
    /// (Useless) Get MaskOBJ from MaskGFX.
    /// </summary>
    public static ConditionalWeakTable<VultureMaskGraphics, VultureMask> maskgfx_to_maskobj_CWT = new ConditionalWeakTable<VultureMaskGraphics, VultureMask>();

   
    public static ConditionalWeakTable<AbstractPhysicalObject, CustomMaskData> abstPhysObj_to_custommask_CWT = new ConditionalWeakTable<AbstractPhysicalObject, CustomMaskData> ();

}
