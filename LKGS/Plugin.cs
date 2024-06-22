using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using System.Collections.Generic;

namespace LKGS;

[BepInPlugin(PluginInfo.kPackageId, PluginInfo.kTitle, PluginInfo.kVersion)]
public class Plugin : BaseUnityPlugin
{
    private static ManualLogSource Log { get; set; }
    private static TimePatches TimeInstance { get; set; }
    private static ConfigEntry<bool> TimeManipulation { get; set; }
    private static ConfigEntry<int> TimeMultiplier { get; set; }

    private void Awake()
    {
        Log = Logger;
        TimeInstance = gameObject.AddComponent<TimePatches>();

        InitializeConfig();
    }

    internal static int TimeValue => TimeMultiplier.Value;

    internal static void L(string message) => Log.LogInfo(message);

    internal static void D(string message) => Log.LogDebug(message);

    private void InitializeConfig()
    {
        TimeManipulation = Config.Bind(
            "Clock",
            "Enable Time Manipulation",
            true,
            new ConfigDescription("Enable time manipulation.", null, new ConfigurationManagerAttributes {Order = 43})
        );
        TimeManipulation.SettingChanged += (_, _) =>
        {
            TimeInstance.enabled = TimeManipulation.Value;
        };

        TimeMultiplier = Config.Bind(
            "Clock",
            "Time Multiplier",
            1,
            new ConfigDescription(
                "Set the time multiplier.",
                new AcceptableValueRange<int>(1, 6),
                new ConfigurationManagerAttributes {ShowRangeAsPercent = false, Order = 41}
            )
        );
        TimeMultiplier.SettingChanged += (_, _) =>
        {
            if (!TimeManipulation.Value) return;
            TimeInstance.UpdateValues(TimeMultiplier.Value);
        };
    }

    internal static readonly KeyValuePair<ConfigDefinition, ConfigDescription> TestConfigClass =
        new(new("Clock", "TestConfigClass"),
        new("description string", new AcceptableValueRange<int>(0, 10), new ConfigurationManagerAttributes {Order = 43}));
}
