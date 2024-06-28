using System;
using System.Collections.Generic;
using UnityEngine;

namespace LKGS;

public class TextureUtils
{
    // Returns true on successful patch, false otherwise
    internal static bool TryReplaceTexture2D(Sprite ogSprite)
    {
        
        Plugin.D($"Attempting to replace texture for {ogSprite.texture.name}");
        Texture2D tex = TextureStore.textureDict[ogSprite.texture.name];

        if (ogSprite.texture.format != tex.format)
        {
            Plugin.L($"INFO! Remaking texture {ogSprite.texture.name}, wants format {ogSprite.texture.format}, have format {tex.format}");

            //https://docs.unity3d.com/ScriptReference/Texture2D.SetPixels.html
            List<TextureFormat> validFormats = new List<TextureFormat>()
            {
                TextureFormat.Alpha8,
                TextureFormat.ARGB32,
                TextureFormat.ARGB4444,
                TextureFormat.BGRA32,
                TextureFormat.R16,
                TextureFormat.R8,
                TextureFormat.RFloat,
                TextureFormat.RG16,
                TextureFormat.RG32,
                TextureFormat.RGB24,
                TextureFormat.RGB48,
                TextureFormat.RGB565,
                TextureFormat.RGB9e5Float,
                TextureFormat.RGBA32,
                TextureFormat.RGBA4444,
                TextureFormat.RGBA64,
                TextureFormat.RGBAFloat,
                TextureFormat.RGBAHalf,
                TextureFormat.RGFloat,
                TextureFormat.RGHalf,
                TextureFormat.RHalf
            };

            if (validFormats.Contains(ogSprite.texture.format))
            {
                Texture2D newTex = new Texture2D(tex.width, tex.height, ogSprite.texture.format, 1, false);
                newTex.SetPixels(tex.GetPixels());
                newTex.Apply();

                TextureStore.textureDict[ogSprite.texture.name] = newTex;
                tex = newTex;
            }
            else
            {
                Plugin.D("Failed to remake texture. Invalid format: " + Enum.GetName(typeof(TextureFormat), ogSprite.texture.format));
            }
        }

        if (tex.width == ogSprite.texture.width && tex.height == ogSprite.texture.height && tex.format == ogSprite.texture.format)
        {
            Graphics.CopyTexture(tex, ogSprite.texture);
            Plugin.D("OK! Replaced Texture " + ogSprite.texture.name + " for Sprite " + ogSprite.name);
            return true;
        }
        else
        {
            Plugin.E($"Failed to replace texture {ogSprite.texture.name} because of dimension or format mismatch. Original Texture: {ogSprite.texture.width}w x {ogSprite.texture.height}h {Enum.GetName(typeof(TextureFormat), ogSprite.texture.format)}, Replacement Texture: {tex.width}w x {tex.height}h {Enum.GetName(typeof(TextureFormat), tex.format)}");
        }

        return false;
    }
}
