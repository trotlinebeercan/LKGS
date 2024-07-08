using System;

using BC = BepInEx.Configuration;
using HL = HarmonyLib;
using UE = UnityEngine;

namespace LKGS;

// as straightforward as can be. you want a menu, we open the menu
//
// debug menu is marked as advanced because:
// "I just hope if people start using that debug menu they realize it might not
//  be 100% bug proof depending on how much they push my logic.😂" - Kay, 2024
//
// camera zoom allows values outside of the default range
// idk if it will break anything but /shrug
//
// see: ScUIManager, ScWndSettingsTabMain, ScWindowResize

public class UIPatch : UE.MonoBehaviour, IPatch
{
    private string kOpenWorkbenchWindowId = "kOpenWorkbenchWindow";
    private string kOpenKitchenWindowId = "kOpenKitchenWindow";
    private string kOpenGameDebugMenuId = "kOpenGameDebugMenu";

    private const float fZoomDeltaScalar = 0.1f;
    private const float fZoomMin = 0.4f;
    private const float fZoomMax = 10f;
    private float fZoomLevelOverride { get; set; }

    public void Initialize()
    {
        ConfigManager.Instance.StartSection("Fun UI Hacks")
            .Create(kOpenWorkbenchWindowId, "Open Workbench", new BC.KeyboardShortcut(UE.KeyCode.None),
                "Opens the workbench from anywhere.",
                null,
                new ConfigurationManagerAttributes {}
            )
            .Create(kOpenKitchenWindowId, "Open Kitchen", new BC.KeyboardShortcut(UE.KeyCode.None),
                "Opens the kitchen from anywhere.",
                null,
                new ConfigurationManagerAttributes {}
            )
            .Create(kOpenGameDebugMenuId, "Game Debug Menu", new BC.KeyboardShortcut(UE.KeyCode.None),
                "Oooo special dev debug menu.",
                null,
                new ConfigurationManagerAttributes { IsAdvanced = true }
            )
        .EndSection("Fun UI Hacks");
    }

    private void Update()
    {
        if (ConfigManager.Instance.GetValue<BC.KeyboardShortcut>(kOpenWorkbenchWindowId).IsDown())
        {
            ScGameManager.Instance?.GetUIManager().OpenCraftMenu(CraftType.Machine);
        }
        if (ConfigManager.Instance.GetValue<BC.KeyboardShortcut>(kOpenKitchenWindowId).IsDown())
        {
            ScGameManager.Instance?.GetUIManager().OpenCraftMenu(CraftType.Kitchen);
        }
        if (ConfigManager.Instance.GetValue<BC.KeyboardShortcut>(kOpenGameDebugMenuId).IsDown())
        {
            ScGameManager.Instance?.GetDebugManager().debugGameFunctions.RevealPanel();
        }
    }

    // click button -> IncreaseZoom -> ChangeZoom -> SetNewZoomLevel -> SetActiveZoomAmount

    [HL.HarmonyPatch(typeof(ScWndSettingsTabMain), nameof(ScWndSettingsTabMain.IncreaseZoom))]
    [HL.HarmonyPrefix]
    private static bool IncreaseZoom_Prefix(ScWndSettingsTabMain __instance)
    {
        // overload the function to modify the increment amount
        __instance.zoomLevel = (float)Math.Round(Math.Clamp(__instance.zoomLevel + fZoomDeltaScalar, fZoomMin, fZoomMax), 1);
		__instance.ChangeZoom();
        return false;
    }

    [HL.HarmonyPatch(typeof(ScWndSettingsTabMain), nameof(ScWndSettingsTabMain.DecreaseZoom))]
    [HL.HarmonyPrefix]
    private static bool DecreaseZoom_Prefix(ScWndSettingsTabMain __instance)
    {
        // overload the function to modify the decrement amount
        __instance.zoomLevel = (float)Math.Round(Math.Clamp(__instance.zoomLevel - fZoomDeltaScalar, fZoomMin, fZoomMax), 1);
		__instance.ChangeZoom();
        return false;
    }

    [HL.HarmonyPatch(typeof(ScWndSettingsTabMain), nameof(ScWndSettingsTabMain.ChangeZoom))]
    [HL.HarmonyPrefix]
    private static bool ChangeZoom_Prefix(ScWndSettingsTabMain __instance)
    {
        // update our cached value
        Plugin.GetStoredPatch<UIPatch>().fZoomLevelOverride = (float)Math.Round(Math.Clamp(__instance.zoomLevel, fZoomMin, fZoomMax), 1);

        // run the necessary UI functions
        __instance.windowResizeScript.SetNewZoomLevel(__instance.zoomLevel);
        __instance.SetButtonState(__instance.btnZoomPlus, __instance.zoomLevel == fZoomMax);
        __instance.SetButtonState(__instance.btnZoomMinus, __instance.zoomLevel == fZoomMin);

        // show the overloaded zoom text in the UI
        var worldZoomText = UE.GameObject.Find("Text_S_Zoom").GetComponent<ScTextUISetup>();
        worldZoomText.textField.SetText($"World Zoom ({__instance.zoomLevel})");
        return false;
    }

    [HL.HarmonyPatch(typeof(ScWindowResize), nameof(ScWindowResize.SetActiveZoomAmount))]
    [HL.HarmonyPrefix]
    private static bool SetActiveZoomAmount_Prefix(ScWindowResize __instance)
    {
        // overload the function to remove all logic and force set activeZoom
        __instance.zoomLevel = Plugin.GetStoredPatch<UIPatch>().fZoomLevelOverride;
        __instance.activeZoom = Plugin.GetStoredPatch<UIPatch>().fZoomLevelOverride;
        if (UE.Mathf.Approximately(__instance.zoomLevel, 0f) || UE.Mathf.Approximately(__instance.activeZoom, 0f))
        {
            Plugin.E($"SetActiveZoomAmount hit with a 0 - z:{__instance.zoomLevel} // a:{__instance.activeZoom}, forcing to 2f");
            Plugin.GetStoredPatch<UIPatch>().fZoomLevelOverride = 2f;
            return SetActiveZoomAmount_Prefix(__instance);
        }
        Plugin.D($"[zoom] - level={__instance.zoomLevel} // active={__instance.activeZoom}");
        return false;
    }
}
