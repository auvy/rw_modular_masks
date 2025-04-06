using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ModularMasks;

using static Naming;
using static Globals;

public static class Hashing
{
    /// <summary>
    /// the part values bias. basically vanilla features are preferred in 40%.
    /// </summary>
    public static double ZERO_BIAS_CHANCE = 0.3;

    //public static bool GLOBAL_SETTINGS.usePrimesForHashing = false;

    public static double UpdateVanillaBias(int newBias)
    {
        ZERO_BIAS_CHANCE = (double)newBias / GLOBAL_SETTINGS.maskPieceVariationCeiling;
        return ZERO_BIAS_CHANCE;
    }

    /// <summary>
    /// Scale the hash to value.
    /// </summary>
    static int ScaleHash(int hash)
    {
        return (hash & 0x7FFFFFFF) % GLOBAL_SETTINGS.maskPieceVariationCeiling;
    }

    /// <summary>
    /// Loop the value if its larger than ceiling.
    /// </summary>
    public static int LoopValueCeilExcl(int value, int ceiling)
    {
        return value >= ceiling ? value % ceiling : value;
    }

    /// <summary>
    /// Hash Function. sha hash of (reversed ID + part type + id)
    /// </summary>
    public static int PartHash(string partName, int maskId)
    {
        //EyesHash = SHAHash(Reverse(maskId.ToString()) + MASK_EYES + maskId.ToString());
        return SHAHash(Reverse(maskId.ToString()) + partName + (maskId.ToString()));
    }
    static int SHAHash(string data)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] idBytes = Encoding.UTF8.GetBytes(data);
            byte[] hashBytes = sha256.ComputeHash(idBytes);
            uint hashValue = BitConverter.ToUInt32(hashBytes, 0) ^ BitConverter.ToUInt32(hashBytes, 4);
            return (int)(hashValue & 0x7FFFFFFF); //always non negative
        }
    }
    public static string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }




    /// <summary>
    /// Get hash of a mask piece.
    /// </summary>
    public static int HashPiece(string partName, int maskId)
    {
        string combined = GLOBAL_SETTINGS.usePrimesForHashing ? $"{maskId}-{ALL_PIECE_DATA[partName].Prime}-{partName}" : $"{maskId}-{ALL_PIECE_DATA[partName].atlasCombinationTag}-{partName}";

        return SHAHash(SHAHash(combined).ToString());
    }


    /// <summary>
    /// From hash, get a number of mask piece within bounds.
    /// </summary>
    public static int GetKrakenPieceWithHash(int hash, int variations, bool bias)
    {
        // this is done so that when i add new pieces, the variation hopefully wont change (it will lol)
        int variation = hash % GLOBAL_SETTINGS.maskPieceVariationCeiling;
        int looped = LoopValueCeilExcl(variation, variations);

        // Normalize to 0-1 range based on part of hash
        double randomFactor = (hash & 0xFFFF) / (double)0xFFFF;

        // Apply bias
        if (!bias || randomFactor > ZERO_BIAS_CHANCE) return looped;

        else return 0;
    }
}
