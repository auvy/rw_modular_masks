using BepInEx.Logging;
using UnityEngine;

namespace ModularMasks;

using static Globals;
using static Constants;

public class Textures
{
    /// <summary>
    /// Cut out from a sheet.
    /// </summary>
    public static Texture2D CropTexture2D(Texture2D source, int x, int y, int width, int height)
    {
        Color[] c = source.GetPixels(x, y, width, height);
        Texture2D croppedStrip = new Texture2D(width, height, TextureFormat.RGBA32, false);
        croppedStrip.SetPixels(c);
        croppedStrip.Apply();

        return croppedStrip;
    }


    public static Texture2D CutOutFromSpritesheet(Texture2D spriteSheet, int variation, int maskSpriteDimension, int rotation = 0)
    {
        // moving down the sheet to selected variant of a piece.
        int y = spriteSheet.height - (maskSpriteDimension * (variation + 1));
        // moving right for a proper rotation
        int x = maskSpriteDimension * rotation;

        return CropTexture2D(spriteSheet, x, y, maskSpriteDimension * MASK_ROTATIONS, maskSpriteDimension);
    }

    public static Texture2D FixFAtlasAlpha(Texture2D result)
    {
        // Get all the pixels from the new texture
        Color[] pixels = result.GetPixels();

        // Iterate over all pixels and swap the R and A channels
        for (int i = 0; i < pixels.Length; i++)
        {
            Color pixel = pixels[i];

            // Swap the Red and Alpha channels
            if (pixel != Color.white)
            {
                float tempR = pixel.r;
                pixel.r = pixel.a;
                pixel.a = tempR;
            }

            // Set the modified pixel back to the texture array
            pixels[i] = pixel;

        }

        // Apply the modified pixels to the texture
        result.SetPixels(pixels);
        result.Apply();

        return result;
    }

    public static Texture2D FAtlasToTexture2D(FAtlas fatlas)
    {
        Texture atlasTexture = fatlas.texture;
        Texture2D result = new Texture2D(atlasTexture.width, atlasTexture.height, TextureFormat.RGBA32, false);
        // Copy texture data from the Futile texture to the new Texture2D
        Graphics.CopyTexture(atlasTexture, result);

        result = FixFAtlasAlpha(result);
        //SaveToPNG(result, fatlas.name);



        return result;
    }


    /// <summary>
    /// Merge pieces of masks into one sprite, overlaying them together pixel by pixel.
    /// Use to combine/overlay all mask pieces into one texture.
    /// </summary>
    public static Texture2D MergeTextures(Texture2D[] layers, int width, int height)
    {
        // Create a new Texture2D to hold the merged result
        Texture2D mergedTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Iterate through each pixel of the merged texture
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Start with a fully transparent color (this will store the final pixel)
                Color finalColor = Color.clear;
                float accumulatedAlpha = 0f; // Accumulated alpha

                // Blend pixels from each layer onto the base texture
                foreach (Texture2D layer in layers)
                {
                    // Get the color of the current pixel in the current layer
                    Color pixelColor = layer.GetPixel(x, y);

                    // If the pixel's alpha is > 0, blend it into the final color
                    if (pixelColor.a > 0f)
                    {
                        // Blend using alpha blending formula
                        float blendAlpha = pixelColor.a * (1 - accumulatedAlpha);
                        finalColor += pixelColor * blendAlpha;
                        accumulatedAlpha += blendAlpha; // Update the accumulated alpha
                    }
                }

                // Ensure that the final alpha doesn't exceed 1 (fully opaque)
                finalColor.a = Mathf.Min(accumulatedAlpha, 1f);

                // Set the color of the pixel in the merged texture
                mergedTexture.SetPixel(x, y, finalColor);
            }
        }

        // Apply the changes to the merged texture
        mergedTexture.Apply();

        // Return the merged texture
        return mergedTexture;
    }



    /// <summary>
    /// AND operation on two layers.
    /// Use it for ScavPaint, where the baseLayer gets cropped by the template (toChangeLayer).
    /// </summary>
    public static Texture2D TrimTexture(Texture2D baseLayer, Texture2D templateLayer, int width, int height)
    {
        // Create a new Texture2D to hold the merged result
        Texture2D mergedTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Iterate through each pixel of the merged texture
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Get the color from the base and the "to change" layer
                Color baseColor = baseLayer.GetPixel(x, y);
                Color toChangeColor = templateLayer.GetPixel(x, y);

                // Start with the base color (keeping its alpha)
                Color finalColor = baseColor;

                // If the template layer has transparency (alpha == 0), we make the base transparent
                if (toChangeColor.a == 0)
                {
                    finalColor.a = 0; // Make this pixel transparent if the template layer says so
                }

                // Set the color of the pixel in the merged texture
                mergedTexture.SetPixel(x, y, finalColor);
            }
        }

        // Apply the changes to the merged texture
        mergedTexture.Apply();

        // Return the merged texture
        return mergedTexture;
    }


    /// <summary>
    /// Remove pixels from one layer by a template.
    /// Use for mask pieces like eyes.
    /// </summary>
    public static Texture2D CarveTexture(Texture2D baseLayer, Texture2D carveLayer, int width, int height)
    {
        // Create a new Texture2D to hold the merged result
        Texture2D mergedTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Iterate through each pixel of the merged texture
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Start with a fully transparent color
                Color finalColor = Color.clear;

                // Get the alpha value of the pixel from the base texture
                Color baseColor = baseLayer.GetPixel(x, y);

                // Get the color of the corresponding pixel from the "TO CHANGE" texture
                Color tocarveColor = carveLayer.GetPixel(x, y);

                if (baseColor.a != 0 && tocarveColor.a == 0)
                {
                    finalColor = baseColor;
                }
                // Set the color of the pixel in the merged texture
                mergedTexture.SetPixel(x, y, finalColor);
            }
        }

        // Apply the changes to the merged texturefa
        mergedTexture.Apply();

        // Return the merged texture
        return mergedTexture;
    }




}
