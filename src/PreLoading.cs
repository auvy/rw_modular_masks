using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModularMasks;


using static Naming;
using static Globals;
//using static Constants;
using static Files;
using static Colors;
using static Hashing;
using static Textures;
using static AuxillaryFatlas;
using static Debug;
using static Constants;



/// <summary>
/// Info about maskpiece type.
/// <para>string Name</para>
/// <para>PieceType Type - Carve, Merge...</para>
/// <para>SourceType Source - Sprite, Colors</para>
/// <para>int Variations - loaded from sprites or color array</para>
/// <para>HASHING</para>
/// <para>string HashingName</para>
/// <para>int Prime - for hashing</para>
/// <para>bool Biased - Hashing. Should generated variant be closer to 0 (default mask)</para>
/// <para>ATLAS NAMING</para>
/// <para>string SpritesheetName - defines spritesheet loading</para>
/// <para>int NameOrder - for naming order in FAtlas</para>
/// <para>string Code - used in naming</para>
/// </summary>
public class Piece
{
    public string Name { get; set; }

    public string HashingName { get; set; }
    public int Prime { get; set; }
    public bool Biased { get; set; }

    public string atlasCombinationTag { get; set; }


    public SourceType Source { get; set; }
    public string SourceName { get; set; }


    public string algorithmName { get; set; }
    public string operation { get; set; } = string.Empty;
    public int order { get; set; }

    public string overrideVariation { get; set; } = null;


    public int Variations { get; set; } = 1;
    public FAtlas Atlas { get; set; }
    public Texture2D AtlasTexture { get; set; }

}

public class BakeAlgorithmStep
{
    public string algorithmName { get; set; } = null;
    public string operation { get; set; } = null;
    public int order { get; set; } = -1;
}

public class SettingsObject
{
    public List<Piece> pieces { get; set; } = null;
    public List<RGBColor> scavPaintColors { get; set; } = null;

    // outer list - mask variation
    // middle list - mask rotation
    // inner list - list of strings
    public List<List<List<PearlStringAttachPoint>>> pearlStringOffsets { get; set; } = null;
    public bool pearlOffsetIsCenter { get; set; } = false;
    public string pearlAttachmentPiece { get; set; } = "ChiefMergeHorns";


    public string kingArrowName { get; set; } = "KingArrow";

    public string maskPiecePrefix { get; set; } = "Modular";
    public string maskAtlasPrefix { get; set; } = "ModularMask";


    public string eliteScavPaintName { get; set; } = "ChiefPaint";

    public string scavPaintColorName { get; set; } = "ColorPaintColor";


    public string chiefMaskBoneColorName { get; set; } = "ColorChiefColor";
    public List<RGBColor> ChiefMaskColors { get; set; } = null;
    public int BlendingColorNumMask { get; set; } = 1;
    public int BlendingColorNumPaint { get; set; } = 1;
    public int BrightnessDivisor { get; set; } = 20;
    public int DarknessDivisor { get; set; } = 20;
    public int vultureMaskSpriteDim { get; set; } = 61;
    public int chieftainMaskSpriteDim { get; set; } = 121;
    public int vanillaBiasHashChance { get; set; } = 30;
    public bool usePrimesForHashing { get; set; } = false;
    public int maskPieceVariationCeiling { get; set; } = 100;


    public bool debugLog { get; set; } = true;
    public bool debugOutputImage { get; set; } = false;

}


public class PearlStringAttachPoint
{
    public float x { get; set; }
    public float y { get; set; }
}
public class PreLoading
{
    public static int LoadPieceSettings(string path)
    {
        GLOBAL_SETTINGS = ReadJson(path);

        PEARL_STRING_POINTS = GLOBAL_SETTINGS.pearlStringOffsets;
        if (GLOBAL_SETTINGS.pearlOffsetIsCenter == false)
        {
            PEARL_STRING_POINTS = OffsetPearlStrings(GLOBAL_SETTINGS.pearlStringOffsets, GLOBAL_SETTINGS.chieftainMaskSpriteDim / 2 );
        }


        UpdateVanillaBias(GLOBAL_SETTINGS.vanillaBiasHashChance);


        // extendable piece objects
        List<Piece> pieces = GLOBAL_SETTINGS.pieces;
        foreach (var piece in pieces)
        {
            try
            {
                ALL_PIECE_DATA.Add(piece.HashingName, piece);
            }
            catch (Exception ex)
            {
                LogWarning(ex);
            }
        }

        // elite scav warpaint colors
        List<RGBColor> colors = GLOBAL_SETTINGS.scavPaintColors;
        foreach (var color in colors)
        {
            SCAV_PAINT_COLORS.Add(rgbToUnity(color.R, color.G, color.B));
        }
        ALL_PIECE_DATA[GLOBAL_SETTINGS.scavPaintColorName].Variations = GLOBAL_SETTINGS.scavPaintColors.Count;


        // chieftain mask colors
        List<RGBColor> chiefmaskcolors = GLOBAL_SETTINGS.ChiefMaskColors;
        foreach (var color in chiefmaskcolors)
        {
            SCAV_CHIEF_BONE_COLORS.Add(rgbToUnity(color.R, color.G, color.B));
        }
        ALL_PIECE_DATA[GLOBAL_SETTINGS.chiefMaskBoneColorName].Variations = GLOBAL_SETTINGS.ChiefMaskColors.Count;

        return 0;
    }




    public static List<List<List<PearlStringAttachPoint>>> OffsetPearlStrings(List<List<List<PearlStringAttachPoint>>> strings, int offset) {
        // outer list - mask variations
        foreach (var list1 in strings)
        {
            // middle list - mask rotations
            foreach (var list2 in list1)
            {
                // inner list - pearl strings
                foreach (var point in list2)
                {
                    point.x -= offset;
                    point.y = offset - point.y;
                }
            }
        }

        return strings;
    }

    public static void LoadAtlases()
    {
        Dictionary<string, int> tempPieceVariations = ALL_PIECE_DATA
        .Where(pair => pair.Value.Source == SourceType.Sprite)
        .ToDictionary(pair => pair.Key, pair => pair.Value.Variations);

        Dictionary<string, FAtlas> tempAtlases = new Dictionary<string, FAtlas>() { };


        foreach (KeyValuePair<string, Piece> entry in ALL_PIECE_DATA)
        {
            if (entry.Value.Source == SourceType.Sprite)
            {
                string spritesheet = entry.Value.SourceName;

                if (!Futile.atlasManager.DoesContainAtlas(spritesheet))
                {
                    FAtlas loaded = Futile.atlasManager.LoadAtlas(BASE_ATLAS_FOLDER + spritesheet);
                    int partsN = loaded._elementsByName.Count;

                    tempAtlases[entry.Key] = loaded;


                    if (loaded == null)
                    {
                        throw new Exception("Custom Vulture sprites NOT LOADED: " + spritesheet);
                    }
                    else
                    {
                        if (partsN % MASK_ROTATIONS == 0)
                        {
                            tempPieceVariations[entry.Key] = partsN / MASK_ROTATIONS;
                            LogInfo("Loaded " + (partsN / MASK_ROTATIONS).ToString() + " " + entry.Key);
                        }
                    }

                }
            }
        }



        // updating old vars
        foreach (KeyValuePair<string, int> entry in tempPieceVariations)
        {
            ALL_PIECE_DATA[entry.Key].Variations = entry.Value;
        }



        // all atlases of all parts in texture form, each index a piece.
        foreach (KeyValuePair<string, FAtlas> entry in tempAtlases)
        {
            ALL_PIECE_DATA[entry.Key].Atlas = tempAtlases[entry.Key];
            ALL_PIECE_DATA[entry.Key].AtlasTexture = FAtlasToTexture2D(entry.Value);
        }
    }


}
