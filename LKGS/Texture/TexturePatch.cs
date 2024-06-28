using HL = HarmonyLib;

using System.IO;
using System.Reflection;
using UnityEngine;

namespace LKGS;

public class TexturePatch : MonoBehaviour, IPatch
{
    public void Initialize()
    {
        string modFolder = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).Name;
        FileLoader.TextureModFolders.Add(modFolder);
        TextureStore.Init();
    }

    [HL.HarmonyPatch(typeof(ScGameManager), nameof(ScGameManager.SetUpManagers))]
    [HL.HarmonyPostfix]
    internal static void SetUpManagers()
    {
        Plugin.D($"Checking on Textures...");
        Sprite[] sprites = Resources.FindObjectsOfTypeAll<Sprite>();
        foreach (Sprite ogSprite in sprites)
        {
            if (ogSprite is not null && ogSprite.texture is not null)
            {
                if (TextureStore.textureDict.ContainsKey(ogSprite.texture.name))
                {
                    TextureUtils.TryReplaceTexture2D(ogSprite);
                }
            }
        }
    }
}
