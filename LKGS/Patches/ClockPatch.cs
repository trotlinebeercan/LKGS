using HL = HarmonyLib;
using BC = BepInEx.Configuration;
using UE = UnityEngine;

namespace LKGS;

// NOTE: this does not work as expected... weird things happen
//       do not try this at home (or do, idgaf)

public class ClockPatch : BasePatch
{
    private string bPauseClockAction = "bPauseClockAction";
    private bool bPauseClock = false;

    public override void OnTriggerUpdate()
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
        if (ConfigManager.Instance.GetValue<BC.KeyboardShortcut>(bPauseClockAction).IsDown())
        {
            Plugin.D("Update detected keyboard shortcut hit");
            bPauseClock = !bPauseClock;
            OnTriggerUpdate();
        }
    }

    public override void OnActiveSceneChanged()
    {
        //Plugin.D("lol");
        //kConfigManager.Get<BC.KeyboardShortcut>(bPauseClockAction);
    }

    public override void Initialize()
    {
        ConfigManager.Instance.StartSection("Clock Management")
            .Create(bPauseClockAction, "Pause Clock", new BC.KeyboardShortcut(UE.KeyCode.F2),
                "Pause the clock completely. Time will not pass. Resets when you enter/leave a room.",
                null,
                new ConfigurationManagerAttributes {}
            )
        .EndSection("Clock Management");
    }

    [HL.HarmonyPatch(typeof(ScTime), nameof(ScTime.PauseClock))]
    [HL.HarmonyPrefix]
    public static void PauseClock(bool pause)
    {
        Plugin.GetStoredPatch<ClockPatch>().bPauseClock = pause;
        //Plugin.GetStoredPatch<ClockPatch>().OnTriggerUpdate();
    }
}