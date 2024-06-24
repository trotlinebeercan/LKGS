using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SM = UnityEngine.SceneManagement;

namespace LKGS;

[BepInEx.BepInPlugin(PluginInfo.kPackageId, PluginInfo.kTitle, PluginInfo.kVersion)]
public class Plugin : BepInEx.BaseUnityPlugin
{
    private static BepInEx.Logging.ManualLogSource Log { get; set; }
    private static HarmonyLib.Harmony Harmony { get; set; }
    private static List<BasePatch> AllPatches = new();

    private void Awake()
    {
        // set the global logger
        Log = Logger;

        // initialize harmonyx
        Harmony = new HarmonyLib.Harmony(PluginInfo.kPackageId);

        // attach the scene manager
        SM.SceneManager.activeSceneChanged += ChangedActiveScene;

        // allocate plugins
        CreateAndStorePatch<TimePatch>(Config);
        CreateAndStorePatch<ClockPatch>(Config);
    }

    private void ChangedActiveScene(SM.Scene current, SM.Scene next)
    {
        L($"ChangedActiveScene | current={current.name}, next={next.name}");
    }

    private void CreateAndStorePatch<T>(BepInEx.Configuration.ConfigFile config)
    {
        BasePatch patch = gameObject.AddComponent(typeof(T)) as BasePatch;
        patch.Initialize(config);
        Harmony.PatchAll(typeof(T));
        AllPatches.Add(patch);
    }

    public static T GetStoredPatch<T>()
    {
        return AllPatches.OfType<T>().First();
    }

    internal static void L(string message) => Log.LogInfo(message);
    internal static void D(string message) => Log.LogDebug(message);
}
