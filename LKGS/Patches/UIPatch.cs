using BC = BepInEx.Configuration;
using UE = UnityEngine;

namespace LKGS;

// as straightforward as can be. you want a menu, we open the menu
//
// debug menu is marked as advanced because:
// "I just hope if people start using that debug menu they realize it might not
//  be 100% bug proof depending on how much they push my logic.😂" - Kay, 2024
//
// see: ScUIManager

public class UIPatch : UE.MonoBehaviour, IPatch
{
    private string kOpenWorkbenchWindowId = "kOpenWorkbenchWindow";
    private string kOpenKitchenWindowId = "kOpenKitchenWindow";
    private string kOpenGameDebugMenuId = "kOpenGameDebugMenu";

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
}
