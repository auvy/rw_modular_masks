using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModularMasks;

public static class Constants
{
    public const int DEFAULT_MASK_SPRITE_COUNT = 3;

    public const int MASK_ROTATIONS = 9;
    public const int DEFAULT_MASK_PIECE = 0;

    public const string DEFAULT_VULTURE_MASK_NAME = "KrakenMask";
    public const string DEFAULT_CHIEF_MASK_NAME = "KingMask";


    public const string BAKE_TYPE_MERGE = "Merge";
    public const string BAKE_TYPE_CARVE = "Carve";
    public const string BAKE_TYPE_MASK = "Mask";

    public const string BAKE_ALGO_VULTUREMASK = "VultureMask";
    public const string BAKE_ALGO_ELITEPAINT = "ElitePaint";
    public const string BAKE_ALGO_CHIEFMASK = "ChiefMask";
    public const string BAKE_ALGO_CHIEFPAINT = "ChiefPaint";


    public const string BASE_ATLAS_FOLDER = "atlases/";
    public const string CACHE_FOLDER = ".cached/";
    public const string SETTINGS_FILE_NAME = "settings.json";
}
