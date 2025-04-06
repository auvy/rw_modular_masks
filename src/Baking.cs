using BepInEx.Logging;
using IL;
using Newtonsoft.Json;
using On;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModularMasks;


//using static Constants;
using static Globals;
using static Naming;
using static Textures;
using static CustomMask;
using static Files;
using static AuxillaryFatlas;
using static Constants;
using static Debug;



public class Baking
{
    public static Texture2D MergeTwoLayers(Texture2D baseLayer, Texture2D templateLayer, int maskSpriteDimension)
    {
        return MergeTextures(new Texture2D[] { baseLayer, templateLayer },  maskSpriteDimension * MASK_ROTATIONS, maskSpriteDimension);
    }
    public static Texture2D CarveTwoLayers(Texture2D baseLayer, Texture2D templateLayer, int maskSpriteDimension)
    {
        return CarveTexture(baseLayer, templateLayer,                       maskSpriteDimension * MASK_ROTATIONS, maskSpriteDimension);
    }
    public static Texture2D MaskTwoLayers(Texture2D baseLayer, Texture2D templateLayer, int maskSpriteDimension)
    {
        return TrimTexture(baseLayer, templateLayer,                        maskSpriteDimension * MASK_ROTATIONS, maskSpriteDimension);
    }


    public static Texture2D BakeStripByAlgorithm(Dictionary<string, IndividualPiece> pieces, int maskSpriteDimension)
    {
        Texture2D resultingLayer = null;

        for (int i = 0; i < pieces.Count; i++)
        {
            var key = pieces.Keys.ElementAt(i);


            Texture2D templateLayer = CutOutFromSpritesheet(ALL_PIECE_DATA[key].AtlasTexture, pieces[key].Variant, maskSpriteDimension);


            string operation = ALL_PIECE_DATA[key].operation;

            if (operation == BAKE_TYPE_MERGE) resultingLayer = MergeTwoLayers(resultingLayer, templateLayer, maskSpriteDimension);
            else if (operation == BAKE_TYPE_CARVE) resultingLayer = CarveTwoLayers(resultingLayer, templateLayer, maskSpriteDimension);
            else if (operation == BAKE_TYPE_MASK) resultingLayer = MaskTwoLayers(resultingLayer, templateLayer, maskSpriteDimension);
            else resultingLayer = templateLayer;
        }

        return resultingLayer;
    }



    public static FAtlas BakeSpriteAtlas(Dictionary<string, IndividualPiece> pieces, string spriteName, int maskSpriteDimension, FAtlas bakedTemplate=null)
    {
        Texture2D bakedTexture = BakeStripByAlgorithm(pieces, maskSpriteDimension);

        if (bakedTemplate != null)
        {
            Texture2D templateMask = FAtlasToTexture2D(bakedTemplate);

            bakedTexture = MaskTwoLayers(bakedTexture, templateMask, maskSpriteDimension);

            SaveToPNG(templateMask, spriteName + "mask");

            SaveToPNG(bakedTexture, spriteName);
        }

        return BakeAndLoadAtlas(bakedTexture, spriteName, maskSpriteDimension);
    }
}
