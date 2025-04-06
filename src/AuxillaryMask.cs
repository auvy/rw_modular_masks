using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using MoreSlugcats;
using RWCustom;

using UnityEngine;
using Newtonsoft.Json.Linq;

namespace ModularMasks;

using static CustomMask;
using static Globals;
using static Baking;
using static Naming;
using static Debug;



public class AuxillaryMask
{

    /// <summary>
    /// Check if VultureMaskGraphics is Elite mask.
    /// </summary> 
    public static bool EliteMaskCheck(VultureMaskGraphics mask)
    {
        bool elite = (mask.overrideSprite != null && mask.overrideSprite.Length != 0);

        return elite;
    }








    public static void UnloadAtlases(AbstractPhysicalObject self)
    {
        // Check if 'self' is of type VultureMask or any of its subclasses
        if (self is VultureMask.AbstractVultureMask)
        {
            VultureMask.AbstractVultureMask vultureMask = (VultureMask.AbstractVultureMask)self;

            if (abstPhysObj_to_custommask_CWT.TryGetValue(vultureMask, out CustomMaskData value))
            {
                UnloadOneAtlas(value.spriteName);

                if (value.isElite)
                {
                    UnloadOneAtlas(value.scavpaintName);
                }
                if (value.isChief)
                {
                    UnloadOneAtlas(value.chiefMaskSpriteName);
                    UnloadOneAtlas(value.chiefPaintSpriteName);
                }
            }
        }
    }


    public static void UnloadOneAtlas(string atlasName)
    {
        Futile.atlasManager.UnloadAtlas(atlasName);

        if (Futile.atlasManager.GetAtlasWithName(atlasName) != null)
        {
            LogWarning("Atlas wasnt unloaded: " + atlasName);
        }
    }


    public static CustomMaskData GetCustomMaskFromGFX(VultureMaskGraphics self)
    {
        CustomMaskData cmaskdata = null;

        if (maskgfx_to_custommask_CWT.TryGetValue(self, out CustomMaskData value))
        {
            cmaskdata = value;
        }
        else
        {
            LogInfo("Couldn't find mask CustomMaskData for a mask.");
        }

        return cmaskdata;
    }


}
