using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Experimental.Rendering;
using System;

namespace LKGS;

internal static class FileLoader
{
    public static List<string> TextureModFolders = new List<string>();

    internal static void LoadTextures()
    {
        Plugin.D($"LoadTextures started, looking at {TextureModFolders.Count} folders");
        foreach (string modName in TextureModFolders)
        {
            string textureDir = getAssetDir(modName, "Textures");
            Plugin.D($"Looking inside texture folder {modName} // {textureDir}");
            try
            {
                foreach (string filepath in Directory.EnumerateFiles(textureDir, "*.*", SearchOption.AllDirectories))
                {
                    Plugin.D("Found file " + Path.GetFileNameWithoutExtension(filepath) + " at " + filepath.Replace(textureDir + "\\", ".\\"));
                    Texture2D texture2D = new Texture2D(2, 2, GraphicsFormat.R8G8B8A8_UNorm, 1, TextureCreationFlags.None);
                    ImageConversion.LoadImage(texture2D, File.ReadAllBytes(filepath));
                    TextureStore.textureDict[Path.GetFileNameWithoutExtension(filepath)] = texture2D;
                }
            }
            catch (Exception e)
            {
                Plugin.E("Error loading Textures. Please make sure you configured the mod folders correctly and it doesn't contain any unrelated files.");
                Plugin.E(e.GetType() + " " + e.Message);
            }
        }
        Plugin.L("Textures loaded successfully.");
    }

    private static string getAssetDir(string modName, string assetType)
    {
        return Path.Combine(BepInEx.Paths.PluginPath, modName, assetType);
    }
}