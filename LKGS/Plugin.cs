using BepInEx;

namespace LKGS
{
    [BepInPlugin(PluginInfo.kPackageId, PluginInfo.kTitle, PluginInfo.kVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.kTitle} is loaded!");
        }
    }
}
