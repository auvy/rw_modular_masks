using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModularMasks;

using static CustomMask;
using static Globals;


public class Naming
{
    /// <summary>
    /// Chief Mask Bone Color
    /// </summary>
    //public static string GLOBAL_SETTINGS.ChiefColorName = "ChiefColorColor";

    /// <summary>
    /// Scavenger Paint Color
    /// </summary>
    //public static string GLOBAL_SETTINGS.scavPaintColorName = "ColorPaintColor";


    /// <summary>
    /// Spritesheet. ModularMask_X_X_X_X_X_X
    /// </summary>
    public static string GetModularMaskName(int[] variations)
    {
        string MaskAtlasName = GLOBAL_SETTINGS.maskAtlasPrefix; // NEW_MASK_NAME_PREFIX;
        for (int maskpart = 0; maskpart < variations.Length; maskpart++)
        {
            MaskAtlasName = MaskAtlasName + "_" + variations[maskpart].ToString();
        }
        return MaskAtlasName;

    }


    /// <summary>
    /// Spritesheet. ModularMask_X_X_X_X_X_X
    /// </summary>
    public static string GetModularMaskNameDict(Dictionary<string, IndividualPiece> maskVariants, string algorithmName)
    {

        // getting a filter of bakepieces
        SortedDictionary<string, IndividualPiece> bakePieces = new SortedDictionary<string, IndividualPiece>(
        maskVariants
        .Where(pair => ALL_PIECE_DATA[pair.Key].algorithmName == algorithmName)
        .ToDictionary(pair => pair.Key, pair => pair.Value),
        StringComparer.OrdinalIgnoreCase);



        string MaskAtlasName = GLOBAL_SETTINGS.maskAtlasPrefix; // NEW_MASK_NAME_PREFIX;

        // applying the bakepieces filter to get a scavmask name
        foreach (KeyValuePair<string, IndividualPiece> entry in bakePieces)
        {
            MaskAtlasName = MaskAtlasName + "_" + ALL_PIECE_DATA[entry.Key].atlasCombinationTag + entry.Value.Variant.ToString();
        }

        return MaskAtlasName;
    }

    public static string GetModularMaskScavPaintName(Dictionary<string, IndividualPiece> maskVariants, string maskSpriteName)
    {
        return maskSpriteName + "_" + ALL_PIECE_DATA[GLOBAL_SETTINGS.eliteScavPaintName].atlasCombinationTag + maskVariants[GLOBAL_SETTINGS.eliteScavPaintName].Variant.ToString();
    }
/*
    public static string GetModularChiefMaskName(Dictionary<string, IndividualPiece> maskVariants)
    {
        return GLOBAL_SETTINGS.maskPiecePrefix + maskVariants[CHIEF_MASK].Variant;
    }*/

/*    public static string GetModularChiefPaintName(Dictionary<string, IndividualPiece> maskVariants)
    {
        return GLOBAL_SETTINGS.maskPiecePrefix + maskVariants[CHIEF_PAINT].Variant;
    }*/

    /// <summary>
    /// Spritesheet. ModularMask_X_X_X_X_X_X_OverlayPaint_X
    /// </summary>
    public static string PaintMaskName(CustomMaskData data)
    {
        return data.spriteName + "_" + data.scavpaintName;
    }


    public static string PaintMaskNameStrings(string maskName, string paintName)
    {
        return maskName + "_" + paintName;
    }


    /// <summary>
    /// name + _r + X
    /// </summary>
    public static string RotatedMaskName(string name, int rotation)
    {
        return name + "_r" + rotation.ToString();
    }
}
