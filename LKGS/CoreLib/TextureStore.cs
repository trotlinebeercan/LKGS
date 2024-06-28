using UnityEngine;
using System.Collections.Generic;

namespace LKGS;

public static class TextureStore
{
    internal static Dictionary<string, Texture2D> textureDict = new Dictionary<string, Texture2D>();
    public static List<Texture2D> changedList = new List<Texture2D>();

    internal static void Init()
    {
        FileLoader.LoadTextures();
    }

    public static void LogAll()
    {
        Plugin.L("Logging all TextureStore entries.");
        foreach (KeyValuePair<string, Texture2D> entry in textureDict)
        {
            Plugin.L("TextureDict Entry: " + entry.Key + " | " + entry.Value);
        }
    }
}