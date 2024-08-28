using System.Reflection;
using MonoMod.RuntimeDetour;

using BepInEx;
using MoreSlugcats;
using RWCustom;
using System.Security.Permissions;
using UnityEngine;
using System;
using System.Collections.Generic;

using System.Runtime.Remoting.Contexts;
using MonoMod.Cil;

using UnityEngine.Networking;

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

using System.Runtime.CompilerServices;
using BepInEx.Logging;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.Remoting.Messaging;
using UnityEngine.Experimental.Rendering;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
// Allows access to private members
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618


namespace modular_masks
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class ModularMasksMain : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "modularmasks";
        public const string PLUGIN_NAME = "Modular Vulture Masks";
        public const string PLUGIN_VERSION = "0.0.6.9";

        public const int MASK_ROTATIONS = 9;
        public const int MASK_SPRITE_DIMENSION = 61;

        public static int MASK_EYES_VARIATIONS = 6;
        public static int MASK_MOUTH_VARIATIONS = 6;
        public static int MASK_FOREHEAD_VARIATIONS = 2;
        public static int MASK_SIDES_VARIATIONS = 4;
        public static int MASK_HORNS_VARIATIONS = 5;
        public static int MASK_PAINT_VARIATIONS = 4;

        public const int DEFAULT_EYES = 0;
        public const int DEFAULT_MOUTH = 0;
        public const int DEFAULT_FOREHEAD = 0;
        public const int DEFAULT_SIDES = 0;
        public const int DEFAULT_HORNS = 0;
        public const int DEFAULT_PAINT = 0;

        public const string MASK_EYES = "Eyes";
        public const string MASK_MOUTH = "Mouth";
        public const string MASK_FOREHEAD = "Forehead";
        public const string MASK_SIDES = "Sides";
        public const string MASK_HORNS = "Horns";
        public const string MASK_PAINT = "Paint";


        public const int MASK_VARIATION_SCALE = 100;


        public static Dictionary<string, int> MaskPieces = new Dictionary<string, int>()
        {
            {MASK_EYES, MASK_EYES_VARIATIONS},
            {MASK_MOUTH, MASK_MOUTH_VARIATIONS},
            {MASK_FOREHEAD, MASK_FOREHEAD_VARIATIONS},
            {MASK_SIDES, MASK_SIDES_VARIATIONS},
            {MASK_HORNS, MASK_HORNS_VARIATIONS},
            {MASK_PAINT, MASK_PAINT_VARIATIONS}
        };


        public class OverseerExtendedColors : OverseerGraphics
        {
            public OverseerExtendedColors(PhysicalObject ow) : base(ow)
            {
            }

            public delegate Color orig_MainColor(OverseerGraphics self);

            public static Color OverseerGraphics_MainColor_get(orig_MainColor orig, OverseerGraphics self)
            {
                return new Color(0.6f, 0.28f, 0.46f);
            }
        }


        static string PreparePartStringToHash(string maskPiece, int id)
        {
            return maskPiece + new string(id.ToString().Reverse().ToArray());
        }
        static int GetHash(string input)
        {
            return input.GetHashCode();
        }

        static int ScaleHash(int hash)
        {
            return (hash & 0x7FFFFFFF) % MASK_VARIATION_SCALE; // Ensure non-negative and scale to 0-100
        }

        static int LoopValue(int value, int ceiling)
        {
            return value > ceiling ? value % ceiling : value;
        }

        static int SHAHash(string data)
        {
            byte[] idBytes = Encoding.UTF8.GetBytes(data);
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(idBytes);
                return BitConverter.ToInt32(hashBytes, 0);
            }
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public class VultureGraphicsMaskPieces
        {
            public delegate int orig_VultureGraphicsTotalSprites(VultureGraphics self);
            public delegate int orig_VultureMaskGraphicsTotalSprites(VultureMaskGraphics self);

            public static string GetPieceAtlasName(string maskPiece)
            {
                return "KrakenMask" + maskPiece;
            }


            public static int GetKrakenPieceRandom(string maskPiece)
            {
                //return GetPieceAtlasName(maskPiece) + UnityEngine.Random.Range(0, MaskPieces[maskPiece]).ToString();
                return UnityEngine.Random.Range(0, MaskPieces[maskPiece]);
            }



            public static int GetKrakenPieceWithHashID(string maskPiece, int id)
            {
                string newStr = PreparePartStringToHash(maskPiece, id);
                int hash = GetHash(newStr);
                int scaledHash = ScaleHash(hash);

                int pieceVariation = LoopValue(scaledHash, MaskPieces[maskPiece] - 1);

                return pieceVariation;
            }

            public static int GetKrakenPieceWithHash(string maskPiece, int hash)
            {
                return LoopValue(ScaleHash(hash), MaskPieces[maskPiece] - 1);
            }

            public static int GetKrakenPaint()
            {
                // unique scav paint
                //return GetPieceAtlasName(MASK_PAINT) + 0.ToString();
                return 0;
            }
        }



        /// vulture CWT

        public static ConditionalWeakTable<Vulture, VultureMask> vultureVultureMaskCWT = new ConditionalWeakTable<Vulture, VultureMask>();
        public static ConditionalWeakTable<VultureMaskGraphics, CustomMaskData> vultureMaskCustomGfxCWT = new ConditionalWeakTable<VultureMaskGraphics, CustomMaskData>();


        public class CustomMaskData
        {
            public int CustomEyes = VultureGraphicsMaskPieces.GetKrakenPieceRandom(MASK_EYES);
            public int CustomMouth = VultureGraphicsMaskPieces.GetKrakenPieceRandom(MASK_MOUTH);
            public int CustomForehead = VultureGraphicsMaskPieces.GetKrakenPieceRandom(MASK_FOREHEAD);
            public int CustomSides = VultureGraphicsMaskPieces.GetKrakenPieceRandom(MASK_SIDES);
            public int CustomHorns = VultureGraphicsMaskPieces.GetKrakenPieceRandom(MASK_HORNS);
            public int CustomPaint = VultureGraphicsMaskPieces.GetKrakenPaint();

            public int EyesHash = -1;
            public int MouthHash = -1;
            public int ForeheadHash = -1;
            public int SidesHash = -1;
            public int HornsHash = -1;
            public int PaintHash = -1;

            public int color = -1;
 

            public FAtlas[] maskRotations = new FAtlas[MASK_ROTATIONS];

            public string spriteName = "KrakenMask";

            public int maskId = -1;

            public int hash = -1;

            public static string GetModularMaskName(int[] variations)
            {
                string MaskAtlasName = "KrakenMask";
                for (int maskpart = 0; maskpart < variations.Length; maskpart++)
                {
                    MaskAtlasName = MaskAtlasName + "_" + variations[maskpart].ToString();
                }
                return MaskAtlasName;

            }

            public CustomMaskData(VultureMask self)
            {

                maskId = self.AbstrMsk.ID.number;

                //color = self.color;

                EyesHash = SHAHash(Reverse(maskId.ToString()) + MASK_EYES + maskId.ToString());
                MouthHash = SHAHash(Reverse(maskId.ToString()) + MASK_MOUTH + maskId.ToString());
                ForeheadHash = SHAHash(Reverse(maskId.ToString()) + MASK_FOREHEAD + maskId.ToString());
                SidesHash = SHAHash(Reverse(maskId.ToString()) + MASK_SIDES + maskId.ToString());
                HornsHash = SHAHash(Reverse(maskId.ToString()) + MASK_HORNS + maskId.ToString());

                /*
                CustomEyes = VultureGraphicsMaskPieces.GetKrakenPieceWithHashID(MASK_EYES, maskId);
                CustomMouth = VultureGraphicsMaskPieces.GetKrakenPieceWithHashID(MASK_MOUTH, maskId);
                CustomForehead = VultureGraphicsMaskPieces.GetKrakenPieceWithHashID(MASK_FOREHEAD, maskId);
                CustomSides = VultureGraphicsMaskPieces.GetKrakenPieceWithHashID(MASK_SIDES, maskId);
                CustomHorns = VultureGraphicsMaskPieces.GetKrakenPieceWithHashID(MASK_HORNS, maskId);
                */
                CustomEyes = VultureGraphicsMaskPieces.GetKrakenPieceWithHash(MASK_EYES, EyesHash);
                CustomMouth = VultureGraphicsMaskPieces.GetKrakenPieceWithHash(MASK_MOUTH, MouthHash);
                CustomForehead = VultureGraphicsMaskPieces.GetKrakenPieceWithHash(MASK_FOREHEAD, ForeheadHash);
                CustomSides = VultureGraphicsMaskPieces.GetKrakenPieceWithHash(MASK_SIDES, SidesHash);
                CustomHorns = VultureGraphicsMaskPieces.GetKrakenPieceWithHash(MASK_HORNS, HornsHash);

                int[] variat = new int[6];
                variat[0] = CustomEyes;
                variat[1] = CustomMouth;
                variat[2] = CustomForehead;
                variat[3] = CustomHorns;
                variat[4] = CustomSides;
                variat[5] = CustomPaint;
                spriteName = GetModularMaskName(variat);


                vultureMaskCustomGfxCWT.Add(self.maskGfx, this);
            }
        }

        public Texture2D CropTexture2D(Texture2D source, int x, int y, int width, int height)
        {
            Color[] c = source.GetPixels(x, y, width, height);
            Texture2D croppedStrip = new Texture2D(width, height, TextureFormat.RGBA32, false);
            croppedStrip.SetPixels(c);
            croppedStrip.Apply();

            return croppedStrip;
        }

        public Texture2D RenderOverlayTextures(Texture2D[] layers, int width, int height)
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

                    // Blend pixels from each layer onto the base texture
                    foreach (Texture2D layer in layers)
                    {
                        // Get the color of the current pixel in the current layer
                        Color pixelColor = layer.GetPixel(x, y);

                        // Blend the pixel color onto the final color, considering alpha values
                        finalColor = Color.Lerp(finalColor, pixelColor, pixelColor.a);
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


        public void SaveTextureToPNG(Texture2D texture, string name)
        {
            try {
                PNGSaver.SaveTextureToFile(texture, Custom.RootFolderDirectory() + name + ".png");
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }


        public List<string> BakedMasks = new List<string>();
        // initially i wanted to cut atlases by strips and merge them, so that i have less atlases.
        //but for that would have to generate a JSON and output it as a file, because FAtlas constructor only accepts path, not JSON string.
        // so i will have to make 9 atlases for 1 mask each time, 9 is for rotations.
        public FAtlas[] PreBakeMask(string[] pieces, int[] variations, CustomMaskData data)
        {
            FAtlas[] newAtlases = new FAtlas[MASK_ROTATIONS];

            string maskAtlasName = data.spriteName;

            Texture2D[] loadedAtlases = new Texture2D[pieces.Length];

            for (int maskpart = 0; maskpart < pieces.Length; maskpart++)
            {
                FAtlasElement atlasWithSprites = Futile.atlasManager.GetElementWithName("KrakenMask" + pieces[maskpart] + "_" + variations[maskpart].ToString() + "_0");
                Texture atlasTexture = atlasWithSprites.atlas.texture;

                Texture2D convertedTexture = new Texture2D(atlasTexture.width, atlasTexture.height, TextureFormat.RGBA32, false);
                Graphics.CopyTexture(atlasTexture, convertedTexture);

                loadedAtlases[maskpart] = convertedTexture;
            }

            for (int rotation = 0; rotation < MASK_ROTATIONS; rotation++)
            {
                Texture2D[] croppedPieces = new Texture2D[pieces.Length];
                for (int atlasi = 0; atlasi < loadedAtlases.Length; atlasi++)
                {
                    int y = loadedAtlases[atlasi].height - (MASK_SPRITE_DIMENSION * (variations[atlasi] + 1));
                    int x = MASK_SPRITE_DIMENSION * rotation;

                    Texture2D pieceAtlas = CropTexture2D(loadedAtlases[atlasi], x, y, MASK_SPRITE_DIMENSION, MASK_SPRITE_DIMENSION);
                    croppedPieces[atlasi] = pieceAtlas;

                }

                Texture2D mergedRotation = RenderOverlayTextures(croppedPieces, MASK_SPRITE_DIMENSION, MASK_SPRITE_DIMENSION);

                if (rotation == 0) SaveTextureToPNG(mergedRotation, maskAtlasName + "_r" + rotation.ToString());


                Texture newAtlasTexture = mergedRotation;
                FAtlas newAtlas = Futile.atlasManager.LoadAtlasFromTexture(maskAtlasName + rotation.ToString(), newAtlasTexture, false);

                data.maskRotations[rotation] = newAtlas;

                if (newAtlas == null)
                {
                    Logger.LogWarning("Custom Vulture sprites NOT LOADED: " + maskAtlasName + "_r" + rotation.ToString());
                }
                else
                {
                    Logger.LogMessage("Baked a new mask sprite: " + maskAtlasName + "_r" + rotation.ToString());
                    //FAtlasElement test = Futile.atlasManager.GetAtlasWithName(maskAtlasName + "_r" + rotation.ToString())._elementsByName[maskAtlasName + "_r" + rotation.ToString()];
                }

                newAtlases[rotation] = newAtlas;
            }

            return newAtlases;

        }



        private void LoadKrakenPieces(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);

            Dictionary<string, int> tempPieces = new Dictionary<string, int>() {
                {MASK_EYES, MASK_EYES_VARIATIONS},
                {MASK_MOUTH, MASK_MOUTH_VARIATIONS},
                {MASK_FOREHEAD, MASK_FOREHEAD_VARIATIONS},
                {MASK_SIDES, MASK_SIDES_VARIATIONS},
                {MASK_HORNS, MASK_HORNS_VARIATIONS},
                {MASK_PAINT, MASK_PAINT_VARIATIONS}
            };


            foreach (KeyValuePair<string, int> entry in MaskPieces)
            {
                string spritesheet = VultureGraphicsMaskPieces.GetPieceAtlasName(entry.Key);
                if (!Futile.atlasManager.DoesContainAtlas(spritesheet))
                {
                    FAtlas loaded = Futile.atlasManager.LoadAtlas("atlases/" + spritesheet);

                    int partsN = loaded._elementsByName.Count;

                    if (loaded == null)
                    {
                        Logger.LogWarning("Custom Vulture sprites NOT LOADED: " + spritesheet);
                    }
                    else
                    {
                        if (partsN % 9 == 0)
                        {
                            tempPieces[entry.Key] = partsN / 9;
                            Logger.LogWarning("Loaded " + (partsN / 9).ToString() + " " + entry.Key);
                        }
                    }
                }
            }

            foreach (KeyValuePair<string, int> entry in tempPieces)
            {
                MaskPieces[entry.Key] = entry.Value;
            }
        }




        BindingFlags propFlags = BindingFlags.Instance | BindingFlags.Public;
        BindingFlags myMethodFlags = BindingFlags.Static | BindingFlags.Public;
        private void OnEnable()
        {

            On.RainWorld.OnModsInit += LoadKrakenPieces;

            On.Vulture.InitiateGraphicsModule += ModularInitiateVultureGraphicsModule;
            //On.Vulture.ctor += VultureConstructor;
            On.VultureGraphics.DrawSprites += ModularDrawVultureSpritesMerged;


            // new stuff
            On.Vulture.DropMask += DropMaskModular;

            //using constructor now
            On.VultureMask.ctor += VultureMaskConstructor;


            On.MoreSlugcats.VultureMaskGraphics.DrawSprites += VultureMaskGraphicsDrawSpritesModular;
        }


        // vulturemask was created
        public void VultureMaskConstructor(On.VultureMask.orig_ctor orig, VultureMask self, AbstractPhysicalObject apo, World world)
        {
            orig(self, apo, world);

            CustomMaskData data;
            if (vultureMaskCustomGfxCWT.TryGetValue(self.maskGfx, out CustomMaskData value))
            {
                data = value;
                Logger.LogWarning("Found mask for some reason: " + value.spriteName);
            }
            else
            {
                data = new CustomMaskData(self);
            }

            // this is allows for mask rotations, but it also applies KrakenArrow color to the whole thing.
            //self.maskGfx.overrideSprite = data.spriteName;



            
            string[] bruh = new string[6];
            bruh[0] = MASK_EYES;
            bruh[1] = MASK_MOUTH;
            bruh[2] = MASK_FOREHEAD;
            bruh[3] = MASK_HORNS;
            bruh[4] = MASK_SIDES;
            bruh[5] = MASK_PAINT;

            int[] variat = new int[6];
            variat[0] = data.CustomEyes;
            variat[1] = data.CustomMouth;
            variat[2] = data.CustomForehead;
            variat[3] = data.CustomHorns;
            variat[4] = data.CustomSides;
            variat[5] = data.CustomPaint;

            if (!BakedMasks.Contains(data.spriteName)) {
                PreBakeMask(bruh, variat, data);
                BakedMasks.Add(data.spriteName);
            }
        }

        public VultureMask CreateAndAddVultureMask(Vulture self)
        {
            AbstractPhysicalObject newObj = new VultureMask.AbstractVultureMask(self.room.world, null, self.abstractPhysicalObject.pos, self.room.game.GetNewID(), self.abstractCreature.ID.RandomSeed, self.IsKing, false, "");

            VultureMask newMask = new VultureMask(newObj, self.room.world);

            vultureVultureMaskCWT.Add(self, newMask);

            return newMask;
        }

        // vulture was made/spawned
        public void VultureConstructor(On.Vulture.orig_ctor orig, Vulture self, AbstractCreature ac, World world)
        {
            orig(self, ac, world);
            if (!self.IsMiros)
            {
                if (!vultureVultureMaskCWT.TryGetValue(self, out VultureMask value))
                {
                    CreateAndAddVultureMask(self);
                }
            }
        }


        // vulture sprites were drawn
        public void ModularInitiateVultureGraphicsModule(On.Vulture.orig_InitiateGraphicsModule orig, Vulture self)
        {
            if (!self.IsMiros)
            {
                if (!vultureVultureMaskCWT.TryGetValue(self, out VultureMask value))
                { 
                    CreateAndAddVultureMask(self); 
                }
            }
            orig(self);
        }








        // mask drawing. i had to hijack it completely because masks were hardcoded.
        public void VultureMaskGraphicsDrawSpritesModular(On.MoreSlugcats.VultureMaskGraphics.orig_DrawSprites orig, VultureMaskGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, UnityEngine.Vector2 camPos)
        {
            


            string maskName = "KrakenMask";

            if (vultureMaskCustomGfxCWT.TryGetValue(self, out CustomMaskData value))
            {
                maskName = value.spriteName;
            }
            else
            {
                Logger.LogWarning("Couldn't find mask CustomMaskData");
            }


            UnityEngine.Vector2 vector = UnityEngine.Vector2.zero;
            UnityEngine.Vector2 v = UnityEngine.Vector3.Slerp(self.lastRotationA, self.rotationA, timeStacker);
            UnityEngine.Vector2 vector2 = UnityEngine.Vector3.Slerp(self.lastRotationB, self.rotationB, timeStacker);
            if (self.overrideRotationVector != null)
            {
                v = self.overrideRotationVector.Value;
            }
            if (self.overrideAnchorVector != null)
            {
                vector2 = self.overrideAnchorVector.Value;
            }
            if (self.overrideDrawVector != null)
            {
                vector = self.overrideDrawVector.Value;
            }
            else if (self.attachedTo != null)
            {
                vector = UnityEngine.Vector2.Lerp(self.attachedTo.firstChunk.lastPos, self.attachedTo.firstChunk.pos, timeStacker);
            }
            float num = rCam.room.Darkness(vector) * (1f - rCam.room.LightSourceExposure(vector)) * 0.8f * (1f - self.fallOffVultureMode);
            float num2 = Custom.VecToDeg(vector2);
            int num3 = Custom.IntClamp(Mathf.RoundToInt(Mathf.Abs(num2 / 180f) * 8f), 0, 8);
            float num4 = self.King ? 1.15f : 1f;
            for (int i = 0; i < (self.King ? 4 : 3); i++)
            {
                if (self.ScavKing)
                {
                    sLeaser.sprites[self.firstSprite + i].element = Futile.atlasManager.GetElementWithName("KingMask" + num3.ToString());
                }
                // classic masks and king masks
                else
                {
                    // elite masks and modular masks.  moved if inside else
                    if (self.overrideSprite != null && self.overrideSprite != "")
                    {
                        sLeaser.sprites[self.firstSprite + i].element = Futile.atlasManager.GetElementWithName(((i != 3) ? self.overrideSprite : "KrakenArrow") + num3.ToString());
                    }
                    // classic masks and king brand
                    else sLeaser.sprites[self.firstSprite + i].element = Futile.atlasManager.GetElementWithName(((i != 3) ? maskName : "KrakenArrow") + num3.ToString());
                }

                sLeaser.sprites[self.firstSprite + i].scaleX = Mathf.Sign(num2) * num4;
                sLeaser.sprites[self.firstSprite + i].anchorY = Custom.LerpMap(Mathf.Abs(num2), 0f, 100f, 0.5f, 0.675f, 2.1f);
                sLeaser.sprites[self.firstSprite + i].anchorX = 0.5f - vector2.x * 0.1f * Mathf.Sign(num2);
                sLeaser.sprites[self.firstSprite + i].rotation = Custom.VecToDeg(v);
                sLeaser.sprites[self.firstSprite + i].x = vector.x - camPos.x;
                sLeaser.sprites[self.firstSprite + i].y = vector.y - camPos.y;
            }
            sLeaser.sprites[self.firstSprite + 1].scaleX *= 0.85f * num4;
            sLeaser.sprites[self.firstSprite + 1].scaleY = 0.9f * num4;
            sLeaser.sprites[self.firstSprite + 2].scaleY = 1.1f * num4;
            sLeaser.sprites[self.firstSprite + 2].anchorY += 0.015f;
            if (self.attachedTo is PlayerCarryableItem && (self.attachedTo as PlayerCarryableItem).blink > 0 && UnityEngine.Random.value < 0.5f)
            {
                for (int j = 0; j < ((!self.King) ? 3 : 4); j++)
                {
                    sLeaser.sprites[self.firstSprite + j].color = new Color(1f, 1f, 1f);
                }
                return;
            }
            self.color = Color.Lerp(Color.Lerp(self.ColorA.rgb, new Color(1f, 1f, 1f), 0.35f * self.fallOffVultureMode), self.blackColor, Mathf.Lerp(0.2f, 1f, Mathf.Pow(num, 2f)));
            sLeaser.sprites[self.firstSprite].color = self.color;
            sLeaser.sprites[self.firstSprite + 1].color = Color.Lerp(self.color, self.blackColor, Mathf.Lerp(0.75f, 1f, num));
            sLeaser.sprites[self.firstSprite + 2].color = Color.Lerp(self.color, self.blackColor, Mathf.Lerp(0.75f, 1f, num));
            if (self.King)
            {
                sLeaser.sprites[self.firstSprite + 3].color = Color.Lerp(Color.Lerp(Color.Lerp(HSLColor.Lerp(self.ColorA, self.ColorB, 0.8f - 0.3f * self.fallOffVultureMode).rgb, self.blackColor, 0.53f), Color.Lerp(self.ColorA.rgb, new Color(1f, 1f, 1f), 0.35f), 0.1f), self.blackColor, 0.6f * num);
            }
            for (int k = 0; k < self.pearlStrings.Count; k++)
            {
                int num5 = k;
                if (Mathf.Sign(vector2.x) < 0f)
                {
                    if (num5 == 0)
                    {
                        num5 = 1;
                    }
                    else if (num5 == 1)
                    {
                        num5 = 0;
                    }
                    else if (num5 == 2)
                    {
                        num5 = 3;
                    }
                    else if (num5 == 3)
                    {
                        num5 = 2;
                    }
                }
                self.pearlStrings[k].layer = self.stringLayers(self.SpriteIndex)[num5];
                self.pearlStrings[k].DrawSprites(sLeaser, rCam, timeStacker, camPos);
            }
        }

        // vulture loses mask
        public void DropMaskModular(On.Vulture.orig_DropMask orig, Vulture self, UnityEngine.Vector2 violenceDir)
        {
            if (!(self.State as Vulture.VultureState).mask)
            {
                return;
            }
            (self.State as Vulture.VultureState).mask = false;

            AbstractPhysicalObject abstractPhysicalObject = vultureVultureMaskCWT.GetValue(self, x => CreateAndAddVultureMask(self)).AbstrMsk;

            self.room.abstractRoom.AddEntity(abstractPhysicalObject);
            abstractPhysicalObject.pos = self.abstractCreature.pos;
            abstractPhysicalObject.RealizeInRoom();
            abstractPhysicalObject.realizedObject.firstChunk.HardSetPosition(self.bodyChunks[4].pos);
            abstractPhysicalObject.realizedObject.firstChunk.vel = self.bodyChunks[4].vel + violenceDir;
            (abstractPhysicalObject.realizedObject as VultureMask).fallOffVultureMode = 1f;
            if (self.killTag != null)
            {
                SocialMemory.Relationship orInitiateRelationship = self.State.socialMemory.GetOrInitiateRelationship(self.killTag.ID);
                orInitiateRelationship.like = -1f;
                orInitiateRelationship.tempLike = -1f;
                orInitiateRelationship.know = 1f;
            }
        }




        // vulture drawing
        public void ModularDrawVultureSpritesMerged(On.VultureGraphics.orig_DrawSprites orig, VultureGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, UnityEngine.Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);



            if (!self.IsMiros)
            {
                //CustomMaskData data = vultureDataCWT.GetValue(self.vulture, x => new CustomMaskData(x));
                VultureMask mask = vultureVultureMaskCWT.GetValue(self.vulture, x =>
                {
                    Logger.LogWarning("Mask was not added to CWT for some reason.");

                    return CreateAndAddVultureMask(self.vulture);
                });


                CustomMaskData data = vultureMaskCustomGfxCWT.GetValue(mask.maskGfx, x =>
                {
                    Logger.LogWarning("Had to make a new custommaskdata too late.");

                    return new CustomMaskData(mask);
                });



                int[] variat = new int[6];
                variat[0] = data.CustomEyes;
                variat[1] = data.CustomMouth;
                variat[2] = data.CustomForehead;
                variat[3] = data.CustomHorns;
                variat[4] = data.CustomSides;
                variat[5] = data.CustomPaint;

                try
                {
                    sLeaser.sprites[self.MaskSprite].element = data.maskRotations[self.headGraphic].elements[0];
                }
                catch (Exception err)
                {
                    Logger.LogError("Now im really confused man: " + err.ToString());
                }

            }

        }

    }
}
