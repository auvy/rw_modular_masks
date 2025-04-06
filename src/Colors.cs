using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


using Color = UnityEngine.Color;
using Graphics = UnityEngine.Graphics;
using Vector2 = UnityEngine.Vector2;


namespace ModularMasks;

using static Globals;


public class RGBColor
{
    public int R { get; set; }
    public int G { get; set; }
    public int B { get; set; }

    public RGBColor(int r, int g, int b)
    {
        R = r;
        G = g;
        B = b;
    }
}

public class Colors
{
    /// <summary>
    /// Int color to Unity float.
    /// </summary>
    public static float urgba(int value)
    {
        return (float)Math.Round((float)value / 255, 3);
    }
    public static int floatToRGB(float component)
    {
        return Mathf.RoundToInt(component * 255);
    }

    public static Color rgbToUnity(int r, int g, int b)
    {
        return new Color(urgba(r), urgba(g), urgba(b));
    }

    public static RGBColor unityToRGB(Color color)
    {
        return new RGBColor(floatToRGB(color.r), floatToRGB(color.g), floatToRGB(color.b));
    }

    private static Color AdjustBrightness(Color color, float factor)
    {
        // Apply brightness factor to each RGB component (clamped between 0 and 1)
        color.r = Mathf.Clamp01(color.r * factor);
        color.g = Mathf.Clamp01(color.g * factor);
        color.b = Mathf.Clamp01(color.b * factor);
        return color;
    }





    public static Color GetBlendedColor(List<Color> colors, int hash, int blendColorAmount)
    {
        int numColors = colors.Count;

        // Number of colors to blend (you can adjust this)
        int numColorsToBlend = blendColorAmount;  // For example, blend all colors, or adjust based on the hash

        // Step 1: Generate indices for the colors to blend using the hash
        List<Color> selectedColors = new List<Color>();
        for (int i = 0; i < numColorsToBlend; i++)
        {
            int colorIndex = (hash + i) % numColors; // Deterministic way to choose colors
            selectedColors.Add(colors[colorIndex]);
        }

        // Step 2: Calculate blend factors using hash to create a variety of blend results
        float[] blendFactors = new float[numColorsToBlend];
        float weightSum = 0f;

        // Use a smoother blend factor distribution (e.g., weights based on a sinusoidal or exponential decay)
        for (int i = 0; i < numColorsToBlend; i++)
        {
            // Simple formula, but with more variation:
            blendFactors[i] = Mathf.Abs(Mathf.Sin((hash + i) * 0.1f));  // Using sine for smooth weight variation
            weightSum += blendFactors[i];
        }

        // Step 3: Normalize the weights to ensure they sum to 1
        for (int i = 0; i < numColorsToBlend; i++)
        {
            blendFactors[i] /= weightSum;  // Normalize to sum to 1
        }

        // Step 4: Blend the selected colors using the weights
        Color blendedColor = Color.black;

        for (int i = 0; i < numColorsToBlend; i++)
        {
            blendedColor.r += selectedColors[i].r * blendFactors[i];
            blendedColor.g += selectedColors[i].g * blendFactors[i];
            blendedColor.b += selectedColors[i].b * blendFactors[i];
        }

        // Step 5: Adjust brightness (deterministically based on hash)
        // You can use the hash value to modify the brightness factor
        float brightnessFactor = 1.0f + (hash % GLOBAL_SETTINGS.BrightnessDivisor) / 100f;
        float darknessFactor = 1.0f - (hash % GLOBAL_SETTINGS.DarknessDivisor) / 100f;

        float combinedFactor = brightnessFactor * darknessFactor;

        blendedColor = AdjustBrightness(blendedColor, combinedFactor);

        // Step 6: Return the blended color
        return blendedColor;
    }

}
