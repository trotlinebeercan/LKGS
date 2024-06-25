using BC = BepInEx.Configuration;
using UE = UnityEngine;

namespace LKGS;

public class UIPatch : UE.MonoBehaviour, IPatch
{
    private string kOpenWorkbenchWindowId = "kOpenWorkbenchWindow";
    private string kOpenKitchenWindowId = "kOpenKitchenWindow";
    private string kOpenGameDebugMenuId = "kOpenGameDebugMenu";

    public void Initialize()
    {
        ConfigManager.Instance.StartSection("Fun UI Hacks")
            .Create(kOpenWorkbenchWindowId, "Open Workbench", new BC.KeyboardShortcut(UE.KeyCode.F3),
                "Opens the workbench from anywhere.",
                null,
                new ConfigurationManagerAttributes {}
            )
            .Create(kOpenKitchenWindowId, "Open Kitchen", new BC.KeyboardShortcut(UE.KeyCode.F4),
                "Opens the kitchen from anywhere.",
                null,
                new ConfigurationManagerAttributes {}
            )
            .Create(kOpenGameDebugMenuId, "Game Debug Menu", new BC.KeyboardShortcut(UE.KeyCode.F5),
                "Oooo special dev debug menu.",
                null,
                new ConfigurationManagerAttributes {}
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
}
