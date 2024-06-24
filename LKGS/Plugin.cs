using System;
using System.Reflection;

using SM = UnityEngine.SceneManagement;

namespace LKGS;

[BepInEx.BepInPlugin(PluginInfo.kPackageId, PluginInfo.kTitle, PluginInfo.kVersion)]
public class Plugin : BepInEx.BaseUnityPlugin
{
    private static BepInEx.Logging.ManualLogSource Log { get; set; }

    private void Awake()
    {
        // set the global logger
        Log = Logger;

        // attach the scene manager
        SM.SceneManager.activeSceneChanged += ChangedActiveScene;

        // allocate plugins
        InitializePatch<TimePatch>(Config);
    }

    private void ChangedActiveScene(SM.Scene current, SM.Scene next)
    {
        L($"ChangedActiveScene | current={current.name}, next={next.name}");
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
