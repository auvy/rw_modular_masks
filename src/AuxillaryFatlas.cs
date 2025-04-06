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
using static Debug;
using static Constants;


public class FramePiece
{
    public int x;
    public int y;
    public int w;
    public int h;
}

public class FrameSourceSize
{
    public int w;
    public int h;
}

/// <summary>
/// A single frame .txt spritesheet
/// <para>frame contains bounds of the sprite</para>
/// </summary>
public class FrameData
{
    public FramePiece frame { get; set; }
    public bool rotated { get; set; }
    public bool trimmed { get; set; }
    public FramePiece spriteSourceSize { get; set; }
    public FrameSourceSize sourceSize { get; set; }
}

/// <summary>
/// Meta of a .txt spritesheet
/// </summary>
public class TextureMeta
{
    public string app { get; set; }
    public string version { get; set; }
    public string image { get; set; }
    public string format { get; set; }
    public FrameSourceSize size { get; set; }
    public string scale { get; set; }
}

/// <summary>
/// Format of a .txt spritesheet
/// </summary>
public class TextureData
{
    public Dictionary<string, FrameData> frames { get; set; }
    public TextureMeta meta { get; set; }
}

internal class AuxillaryFatlas
{
    /// <summary>
    /// Generate JSON for one baked strip
    /// </summary>
    public static object GenerateMaskStripObject(string name, int maskSpriteDimension)
    {
        var frames = new Dictionary<string, FrameData>();

        for (int rot = 0; rot < MASK_ROTATIONS; rot++)
        {
            var frameData = new FrameData
            {
                frame = new FramePiece
                {
                    x = maskSpriteDimension * rot,
                    y = maskSpriteDimension * 0,
                    w = maskSpriteDimension,
                    h = maskSpriteDimension
                },
                rotated = false,
                trimmed = true,
                spriteSourceSize = new FramePiece
                {
                    x = 0,
                    y = 0,
                    w = maskSpriteDimension,
                    h = maskSpriteDimension
                },
                sourceSize = new FrameSourceSize
                {
                    w = maskSpriteDimension,
                    h = maskSpriteDimension
                }
            };

            string key = $"{name}_{rot}.png";
            frames[key] = frameData;
        }


        var meta = new TextureMeta
        {
            app = "https://www.codeandweb.com/texturepacker",
            version = "1.0",
            image = name + ".png",
            format = "RGBA8888",
            size = new FrameSourceSize { w = maskSpriteDimension * MASK_ROTATIONS, h = maskSpriteDimension },
            scale = "1"
        };

        var textureData = new TextureData
        {
            frames = frames,
            meta = meta
        };


        return textureData;
    }


    /// <summary>
    /// FAtlas element code for making new atlas elements.
    /// </summary>
    public static FAtlasElement NewElement(int rot, Texture2D texture, string name, int maskSpriteDimension)
    {
        FAtlasElement fAtlasElement = new FAtlasElement();

        fAtlasElement.name = $"{name}_{rot}.png";
        fAtlasElement.indexInAtlas = rot;

        // like in txt files
        int frame_x = maskSpriteDimension * rot;
        int frame_y = maskSpriteDimension * 0;
        int frame_w = maskSpriteDimension;
        int frame_h = maskSpriteDimension;

        bool rotated = false;
        bool trimmed = true;

        int spriteSourceSize_x = 0;
        int spriteSourceSize_y = 0;
        int spriteSourceSize_w = maskSpriteDimension;
        int spriteSourceSize_h = maskSpriteDimension;

        int sourceSize_w = maskSpriteDimension;
        int sourceSize_h = maskSpriteDimension;


        // like in fatlas

        float resourceScaleInverse = Futile.resourceScaleInverse;

        fAtlasElement.isTrimmed = trimmed;

        //IDictionary obj2 = (IDictionary)dictionary2["frame"];
        float num3 = float.Parse(frame_x.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);
        float num4 = float.Parse(frame_y.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);
        float num5 = float.Parse(frame_w.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);
        float num6 = float.Parse(frame_h.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);
        Rect rect = (fAtlasElement.uvRect = new Rect(num3 / texture.width, (texture.height - num4 - num6) / texture.height, num5 / texture.width, num6 / texture.height));
        fAtlasElement.uvTopLeft.Set(rect.xMin, rect.yMax);
        fAtlasElement.uvTopRight.Set(rect.xMax, rect.yMax);
        fAtlasElement.uvBottomRight.Set(rect.xMax, rect.yMin);
        fAtlasElement.uvBottomLeft.Set(rect.xMin, rect.yMin);
        //IDictionary dictionary3 = (IDictionary)dictionary2["sourceSize"];
        fAtlasElement.sourcePixelSize.x = float.Parse(sourceSize_w.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);
        fAtlasElement.sourcePixelSize.y = float.Parse(sourceSize_h.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);
        fAtlasElement.sourceSize.x = fAtlasElement.sourcePixelSize.x * resourceScaleInverse;
        fAtlasElement.sourceSize.y = fAtlasElement.sourcePixelSize.y * resourceScaleInverse;
        //IDictionary obj3 = (IDictionary)dictionary2["spriteSourceSize"];
        float x = float.Parse(spriteSourceSize_x.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture) * resourceScaleInverse;
        float y = float.Parse(spriteSourceSize_y.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture) * resourceScaleInverse;
        float width = float.Parse(spriteSourceSize_w.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture) * resourceScaleInverse;
        float height = float.Parse(spriteSourceSize_h.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture) * resourceScaleInverse;
        fAtlasElement.sourceRect = new Rect(x, y, width, height);

        return fAtlasElement;
    }



    /// <summary>
    /// Make new atlas in memory.
    /// </summary>
    public static FAtlas BakeAndLoadAtlas(Texture2D bakedTexture, string atlasName, int maskSpriteDimension)
    {

        if (Futile.atlasManager._allElementsByName.ContainsKey(atlasName + "_0.png"))
        {
            return Futile.atlasManager.GetAtlasWithName(atlasName);
        }

        FAtlas idknew = new FAtlas(atlasName, bakedTexture, FAtlasManager._nextAtlasIndex++, true);

        idknew._elements.Clear();
        idknew._elementsByName.Clear();

        for (int rot = 0; rot < MASK_ROTATIONS; rot++)
        {
            FAtlasElement nelement = NewElement(rot, bakedTexture, atlasName, maskSpriteDimension);

            idknew.elements.Add(nelement);
            idknew._elementsByName.Add(nelement.name, nelement);
        }
        try
        {
            Futile.atlasManager.AddAtlas(idknew);
        }
        catch (Exception ex)
        {
            LogError(ex.ToString());
        }



        return idknew;
    }



    /// <summary>
    /// Saving the spritesheet, and the .txt to it
    /// </summary>
    public static bool OutputAtlas(Texture2D atlasStrip, string atlasName, int maskSpriteDimension)
    {
        // exporting the mask for debugging.
        SaveTextureToPNG(atlasStrip, atlasName);

        object maskatlastext = GenerateMaskStripObject(atlasName, maskSpriteDimension);
        OutputObjToJson(CACHE_PATH + atlasName + ".txt", maskatlastext);

        if (FileExists(CACHE_PATH + atlasName + ".txt") && FileExists(CACHE_PATH + atlasName + ".png")) return true;

        return false;
    }

    public static FAtlas BakeAndLoadAtlasFromFile(Texture2D bakedTexture, string atlasName, int maskSpriteDimension)
    {
        // saving it in .cached
        if (OutputAtlas(bakedTexture, atlasName, maskSpriteDimension))
        {
            FAtlas loaded = Futile.atlasManager.LoadAtlas(CACHE_FOLDER + atlasName);

            if (loaded == null)
            {
                throw new Exception("Futile AtlasManager failed to load mask sprites: " + atlasName);
                // here i shouldve assign like default mask names or something.
            }

            return loaded;
        }
        else
        {
            throw new Exception("No mask atlas files found for " + atlasName);
        }
    }

}
