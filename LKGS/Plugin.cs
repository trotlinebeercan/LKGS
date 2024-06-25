using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using BC = BepInEx.Configuration;
using HL = HarmonyLib;
using SM = UnityEngine.SceneManagement;

namespace LKGS;

[BepInEx.BepInPlugin(PluginInfo.kPackageId, PluginInfo.kTitle, PluginInfo.kVersion)]
public class Plugin : BepInEx.BaseUnityPlugin
{
    protected static BepInEx.Logging.ManualLogSource kLog { get; private set; }
    protected static HL.Harmony kHarmony { get; private set; }
    protected static List<BasePatch> kAllPatches = new();

    private void Awake()
    {
        Logger.LogInfo($"Hello world, from {PluginInfo.kTitle}!");

        // set the global logger
        kLog = Logger;

        // initialize lazy config manager
        ConfigManager.Instance.Initialize(Config);

        // initialize harmonyx
        kHarmony = new HL.Harmony(PluginInfo.kPackageId);

        // attach the scene manager
        SM.SceneManager.activeSceneChanged += ChangedActiveScene;

        // allocate plugins
        CreateAndStorePatch<TimePatch>();
        CreateAndStorePatch<CharacterPatch>();

        // disabling this for now, until I can figure out what is going on
        CreateAndStorePatch<ClockPatch>();
    }

    private void ChangedActiveScene(SM.Scene current, SM.Scene next)
    {
        D($"ChangedActiveScene | current={current.name}, next={next.name}");
        kAllPatches.ForEach(p => p.OnActiveSceneChanged());
    }

    private void CreateAndStorePatch<T>() where T : BasePatch, new()
    {
        bool isUnityObject = typeof(T).IsSubclassOf(typeof(UnityEngine.MonoBehaviour));
        T patch = isUnityObject ? (T)(object)gameObject.AddComponent(typeof(T)) : new();
        patch.Initialize();
        kHarmony?.PatchAll(typeof(T));
        kAllPatches.Add(patch);

         D($"Created {typeof(T)} - isUnityObject={isUnityObject}");
    }

    public static T GetStoredPatch<T>()
    {
        return kAllPatches.OfType<T>().First();
    }

    internal static void L(string message) => kLog.LogInfo(message);
    internal static void D(string message) => kLog.LogDebug(message);
}
