using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SM = UnityEngine.SceneManagement;

namespace LKGS;

[BepInEx.BepInPlugin(PluginInfo.kPackageId, PluginInfo.kTitle, PluginInfo.kVersion)]
public class Plugin : BepInEx.BaseUnityPlugin
{
    private static BepInEx.Logging.ManualLogSource kLog { get; set; }
    private static HarmonyLib.Harmony kHarmony { get; set; }
    private static ConfigManager kConfigManager { get; set; }
    private static List<BasePatch> kAllPatches = new();

    private void Awake()
    {
        // set the global logger
        kLog = Logger;

        // initialize config manager
        kConfigManager = new(Config);

        // initialize harmonyx
        kHarmony = new HarmonyLib.Harmony(PluginInfo.kPackageId);

        // attach the scene manager
        SM.SceneManager.activeSceneChanged += ChangedActiveScene;

        // allocate plugins
        CreateAndStorePatch(p => new TimePatch(kConfigManager));
        CreateAndStorePatch(p => new CharacterPatch(kConfigManager));

        // disabling this for now, until I can figure out what is going on
        // CreateAndStorePatch<ClockPatch>(Config);
    }

    private void ChangedActiveScene(SM.Scene current, SM.Scene next)
    {
        L($"ChangedActiveScene | current={current.name}, next={next.name}");
    }

    private void CreateAndStorePatch<T>(Func<ConfigManager,T> alloc) where T : BasePatch
    {
        T patch = alloc(kConfigManager);
        kHarmony?.PatchAll(typeof(T));
        kAllPatches.Add(patch);
    }

    public static T GetStoredPatch<T>()
    {
        return kAllPatches.OfType<T>().First();
    }

    internal static void L(string message) => kLog.LogInfo(message);
    internal static void D(string message) => kLog.LogDebug(message);
}
