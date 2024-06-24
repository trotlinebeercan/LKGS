using System;
using System.Reflection;

namespace LKGS;

[BepInEx.BepInPlugin(PluginInfo.kPackageId, PluginInfo.kTitle, PluginInfo.kVersion)]
public class Plugin : BepInEx.BaseUnityPlugin
{
    private static BepInEx.Logging.ManualLogSource Log { get; set; }

    private void Awake()
    {
        // set the global logger
        Log = Logger;

        // allocate plugins
        InitializePatch<TimePatch>(Config);
    }

    private BasePatch InitializePatch<T>(BepInEx.Configuration.ConfigFile config)
    {
        BasePatch patch = gameObject.AddComponent(typeof(T)) as BasePatch;
        patch.Initialize(config);
        return patch;
    }

    internal static void L(string message) => Log.LogInfo(message);
    internal static void D(string message) => Log.LogDebug(message);
}
