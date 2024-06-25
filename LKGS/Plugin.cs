using System;
using System.Collections.Generic;
using System.IO;
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
    protected static List<IPatch> kAllPatches = new();

    private void Awake()
    {
        Logger.LogInfo($"Hello world, from {PluginInfo.kTitle}!");

        // set the global logger
        kLog = Logger;

#if false // enable to delete the config file and force defaults (for testing)
        File.Delete(Config.ConfigFilePath);
        Config.Do(kvp => kvp.Value.BoxedValue = kvp.Value.DefaultValue);
        Config.Save();
#endif

        // initialize lazy config manager
        ConfigManager.Instance.Initialize(Config);

        // initialize harmonyx
        kHarmony = new HL.Harmony(PluginInfo.kPackageId);

        // attach the scene manager
        SM.SceneManager.activeSceneChanged += ChangedActiveScene;

        // allocate plugins
        CreateAndStorePatch<CharacterPatch>();
        CreateAndStorePatch<TimePatch>();
        CreateAndStorePatch<UIPatch>();
        CreateAndStorePatch<ClockPatch>();
    }

    private void ChangedActiveScene(SM.Scene current, SM.Scene next)
    {
        D($"ChangedActiveScene | current={current.name}, next={next.name}");
        kAllPatches.ForEach(p => p.OnActiveSceneChanged());
    }

    private void CreateAndStorePatch<T>() where T : IPatch, new()
    {
        // we don't need to add the patch as a game component if it doesn't
        // require Unity hooks. we can still access Unity classes regardless
        bool isUnityObject = typeof(T).IsSubclassOf(typeof(UnityEngine.MonoBehaviour));
        T patch = isUnityObject ? (T)(object)gameObject.AddComponent(typeof(T)) : new();

        // do all the inits
        patch.Initialize();
        kHarmony?.PatchAll(typeof(T));

        // store locally for GetStoredPatch<>
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
