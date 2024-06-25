using HL = HarmonyLib;
using BC = BepInEx.Configuration;
using UE = UnityEngine;

namespace LKGS;

// NOTE: this does not work as expected... weird things happen
//       do not try this at home (or do, idgaf)

public class ClockPatch : UE.MonoBehaviour, IPatch
{
    private string bEnablePauseClockToggleId = "bEnablePauseClockToggle";
    private string kPauseClockActionId = "kPauseClockAction";
    private bool bPauseClock = false;

    public void OnTriggerUpdate()
    {
        // don't try to access something that does not exist
        var timeManager = ScGameManager.Instance?.GetTimeManager();
        if (timeManager == null) return;

        // don't try to modify a setting that doesn't need to change
        if (timeManager.pauseClock == bPauseClock) return;

        // do the thing
        Plugin.D($"doing the thing {bPauseClock}");
        timeManager.PauseClock(bPauseClock);
    }

    private void Update()
    {
        if (ConfigManager.Instance.GetValue<bool>(bEnablePauseClockToggleId) &&
            ConfigManager.Instance.GetValue<BC.KeyboardShortcut>(kPauseClockActionId).IsDown())
        {
            bPauseClock = !bPauseClock;
            OnTriggerUpdate();
        }
    }

    public void Initialize()
    {
        ConfigManager.Instance.StartSection("Clock Management")
            .Create(bEnablePauseClockToggleId, "Allow Pause Clock", false,
                "Allow the clock to be paused.",
                null,
                new ConfigurationManagerAttributes{}
            )
            .Create(kPauseClockActionId, "Pause Clock Shortcut", new BC.KeyboardShortcut(UE.KeyCode.F2),
                "Pause the clock, time will not pass. Resets when you enter/leave a room.",
                null,
                new ConfigurationManagerAttributes {}
            )
        .EndSection("Clock Management");
    }

    [HL.HarmonyPatch(typeof(ScTime), nameof(ScTime.PauseClock))]
    [HL.HarmonyPrefix]
    public static void PauseClock(bool pause)
    {
        // reset the state of the clock when the system changes it
        Plugin.GetStoredPatch<ClockPatch>().bPauseClock = pause;
    }
}