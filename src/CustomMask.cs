using BepInEx.Logging;
using IL.MoreSlugcats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace ModularMasks;


//using static Constants;
using static Globals;
using static Naming;
using static Hashing;
using static Colors;
using static HooksMask;
using static AuxillaryMask;
using static Baking;
using static Debug;
using static Constants;


public enum BakingAlgoType
{
    VultureMask, 
    ElitePaint,
    ChiefMask, 
    ChiefPaint
}


public enum SourceType { 
    Sprite,
    Colors
}


/// <summary>
/// <para>int Hash</para>
/// <para>int Variant</para>
/// </summary>
public class IndividualPiece
{
    public int Hash { get; set; }
    public int Variant { get; set; }
}




public class CustomMask
{




    public class CustomMaskData
    {
        /// <summary>
        /// Pair of MaskCarve and a object with Hash and Variation of the thing
        /// </summary>
        public Dictionary<string, IndividualPiece> MaskVariants = new Dictionary<string, IndividualPiece>();

        /*        /// <summary>
                /// Array of baked mask rotation atlases. 
                /// </summary>
                public FAtlasElement[] maskRotationsElements = new FAtlasElement[MASK_ROTATIONS];

                /// <summary>
                /// Array of baked mask paint rotation atlases.
                /// </summary>
                public FAtlasElement[] paintRotationsElements = new FAtlasElement[MASK_ROTATIONS];
        */



        /// <summary>
        /// Array of baked mask rotation atlases. 
        /// </summary>
        public FAtlas maskRotationsAtlas = null;

        /// <summary>
        /// Array of baked mask paint rotation atlases.
        /// </summary>
        public FAtlas paintRotationsAtlas = null;

        /// <summary>
        /// Array of baked mask paint rotation atlases.
        /// </summary>
        public FAtlasElement[] arrowRotationsElements = new FAtlasElement[MASK_ROTATIONS];
        public FAtlasElement[] chiefRotationsElements = new FAtlasElement[MASK_ROTATIONS];
        public FAtlasElement[] chiefPaintRotationsElements = new FAtlasElement[MASK_ROTATIONS];




        /// <summary>
        /// Array of baked chief  rotation atlases.
        /// </summary>
        public FAtlas chiefMaskRotationsAtlas = null;


        /// <summary>
        /// Array of baked chief  rotation atlases.
        /// </summary>
        public FAtlas chiefPaintRotationsAtlas = null;






        public List<MoreSlugcats.VultureMaskGraphics.CosmeticPearlString> pearlStringsCustom = new List<MoreSlugcats.VultureMaskGraphics.CosmeticPearlString>();

        public string spriteName = DEFAULT_VULTURE_MASK_NAME;
        public string scavpaintName = "";

        public string chiefMaskSpriteName = DEFAULT_CHIEF_MASK_NAME;
        public string chiefPaintSpriteName = "";


        // mask item game object id.
        public int maskId = -1;

        public bool isElite = false;
        public bool isChief = false;

        public Color maskColor = Color.white;
        public Color paintColor = Color.white;



        // constructor
        /// <summary>
        /// Constructor by abstobj ID only. Use to preinitiate things before MaskGraphics.
        /// </summary>
        public CustomMaskData(int abstObjId)
        {
            maskId = abstObjId;

            // predicting all the custom stuff. no matter the mask type
            foreach (KeyValuePair<string, Piece> entry in ALL_PIECE_DATA)
            {
                IndividualPiece newestPiece = new IndividualPiece();
                newestPiece.Hash = HashPiece(entry.Key, maskId);
                newestPiece.Variant = GetKrakenPieceWithHash(newestPiece.Hash, entry.Value.Variations, entry.Value.Biased);

                MaskVariants[entry.Key] = newestPiece;


            }

            string maskCombo = "";

            foreach (KeyValuePair<string, Piece> entry in ALL_PIECE_DATA)
            {
                if (entry.Value.overrideVariation != null)
                {
                    MaskVariants[entry.Key].Variant = MaskVariants[entry.Value.overrideVariation].Variant;
                }

                maskCombo += entry.Value.atlasCombinationTag + MaskVariants[entry.Key].Variant + "_";
            }
            LogInfo("VultureMask " + maskId + ": " + maskCombo);




            // usual mask stuff
            spriteName = GetModularMaskNameDict(MaskVariants, BAKE_ALGO_VULTUREMASK);
            scavpaintName = GetModularMaskScavPaintName(MaskVariants, spriteName);

            chiefMaskSpriteName = GetModularMaskNameDict(MaskVariants, BAKE_ALGO_CHIEFMASK);
            chiefPaintSpriteName = chiefMaskSpriteName + "_" + GetModularMaskNameDict(MaskVariants, BAKE_ALGO_CHIEFPAINT);

/*            // chieftain mask stuff
            chiefMaskSpriteName = GetModularChiefMaskName(MaskVariants);
            chiefPaintSpriteName = GetModularChiefPaintName(MaskVariants);*/

            maskColor = GetBlendedColor(SCAV_CHIEF_BONE_COLORS, MaskVariants[GLOBAL_SETTINGS.chiefMaskBoneColorName].Hash, GLOBAL_SETTINGS.BlendingColorNumMask);
            paintColor = GetBlendedColor(SCAV_PAINT_COLORS, MaskVariants[GLOBAL_SETTINGS.scavPaintColorName].Hash, GLOBAL_SETTINGS.BlendingColorNumPaint);


            // easier to have an array of FAtlasElements rather than searching all the time or cutting out a variant from spritesheet
            for (int i = 0; i < MASK_ROTATIONS; i++)
            {
                arrowRotationsElements[i] = Futile.atlasManager.GetElementWithName(ALL_PIECE_DATA[GLOBAL_SETTINGS.kingArrowName].SourceName + "_" + MaskVariants[GLOBAL_SETTINGS.kingArrowName].Variant.ToString() + "_" + i.ToString());

/*                chiefRotationsElements[i] = Futile.atlasManager.GetElementWithName(ALL_PIECE_DATA[CHIEF_MASK].SourceName + "_" + MaskVariants[CHIEF_MASK].Variant.ToString() + "_" + i.ToString());
                chiefPaintRotationsElements[i] = Futile.atlasManager.GetElementWithName(ALL_PIECE_DATA[CHIEF_PAINT].SourceName + "_" + MaskVariants[CHIEF_MASK].Variant.ToString() + "_" + i.ToString());*/
            }



            // changing global parameter to be picked up by vulture mask initializer
            CURRENT_TEMP_PEARLSTRING_VARIATION = MaskVariants[GLOBAL_SETTINGS.pearlAttachmentPiece].Variant;
        }

        /// <summary>
        /// Initialize CustomMask
        /// </summary>
        public static CustomMaskData CompleteInitCustomMaskData(VultureMask vmask)
        {
            CustomMaskData ok = new CustomMaskData(vmask.AbstrMsk.ID.number);

            ok = ok.FinishInitiating(vmask);

            return ok;
        }

        /// <summary>
        /// Split for custom pearlstrings. But im not sure they work.
        /// </summary>
        public CustomMaskData FinishInitiating(VultureMask self) 
        {
            isElite = EliteMaskCheck(self.maskGfx);
            isChief = self.AbstrMsk.scavKing;

            if (isChief)
            {
                pearlStringsCustom = ReInitiatePearlStrings(self.maskGfx);
            }

            maskgfx_to_custommask_CWT.Add(self.maskGfx, this);

            return this;
        }


        /// <summary>
        /// Bake mask FAtlas
        /// </summary>
        public bool BakeNeededAtlases(VultureMask mask) 
        {
            if (!mask.maskGfx.ScavKing)
            {
                maskRotationsAtlas = BaseBake(BAKE_ALGO_VULTUREMASK, spriteName, GLOBAL_SETTINGS.vultureMaskSpriteDim);

                if (EliteMaskCheck(mask.maskGfx))
                {
                    paintRotationsAtlas = BaseBake(BAKE_ALGO_ELITEPAINT, scavpaintName, GLOBAL_SETTINGS.vultureMaskSpriteDim, maskRotationsAtlas);
                }
            }
            else
            {
                chiefMaskRotationsAtlas = BaseBake(BAKE_ALGO_CHIEFMASK, chiefMaskSpriteName, GLOBAL_SETTINGS.chieftainMaskSpriteDim);
                chiefPaintRotationsAtlas = BaseBake(BAKE_ALGO_CHIEFPAINT, chiefPaintSpriteName, GLOBAL_SETTINGS.chieftainMaskSpriteDim, chiefMaskRotationsAtlas);
            }
            return true;
        }



        public static Dictionary<string, IndividualPiece> GetSortedAlgoPieces(Dictionary<string, IndividualPiece> maskVars, string algorithmName)
        {
            // Sort the dictionary entries by the .order property in the values
            var sortedItems = maskVars
                .Where(pair => ALL_PIECE_DATA[pair.Key].algorithmName == algorithmName)
                .OrderBy(pair => ALL_PIECE_DATA[pair.Key].order)  // Sort by the .order property
                .ToDictionary(pair => pair.Key, pair => pair.Value);  // Convert to Dictionary

            return sortedItems;
        }




        public FAtlas BaseBake(string bakingAlgorithm, string spriteName, int maskDimensions, FAtlas maskAtlas = null)
        {
            Dictionary<string, IndividualPiece> sortedMaskPieces = GetSortedAlgoPieces(this.MaskVariants, bakingAlgorithm);

            FAtlas bakedSprites = BakeSpriteAtlas(sortedMaskPieces, spriteName, maskDimensions, maskAtlas);

            return bakedSprites;

        }




        public static FAtlasElement GetCustomMaskRotated(CustomMaskData data, int number)
        {
            //return data.maskRotationsElements[number];
            return data.maskRotationsAtlas._elements[number];
        }
        public static FAtlasElement GetCustomMaskPaintRotated(CustomMaskData data, int number)
        {
            //return data.paintRotationsElements[number];
            return data.paintRotationsAtlas._elements[number];

        }

        public static FAtlasElement GetCustomChiefMaskRotated(CustomMaskData data, int number)
        {
            //return data.chiefRotationsElements[number];
            return data.chiefMaskRotationsAtlas._elements[number];
        }
        public static FAtlasElement GetCustomChiefPaintRotated(CustomMaskData data, int number)
        {
            //return data.chiefPaintRotationsElements[number];
            return data.chiefPaintRotationsAtlas._elements[number];
        }

        public static FAtlasElement GetCustomKingArrowRotated(CustomMaskData data, int number)
        {
            return data.arrowRotationsElements[number];
        }
    }


    public static Vector2 StringCoordinate(PearlStringAttachPoint pearlString)
    {
        return new Vector2(pearlString.x, pearlString.y);
    }

    public static Vector2[] GetStringOffsets(int variation, int rotation)
    {
        List<PearlStringAttachPoint> pearlStringList = PEARL_STRING_POINTS[variation][rotation];

        return pearlStringList.Select(StringCoordinate).ToArray();
    }
}
