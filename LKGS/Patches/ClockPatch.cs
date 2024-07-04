using HL = HarmonyLib;
using BC = BepInEx.Configuration;
using UE = UnityEngine;

namespace LKGS;

// logic here is:
//    game calls PauseClock(bool) => we cache this
//    when you trigger our update to pause the clock, we invert the cached state
//    after, we call PauseClock(!bool), essentially
//    we check if the cached value is equal to the stored value in ScTimeManager
//    if so, we return and do nothing to prevent stack overflow via recursion
//    I don't like this, ergo, it is marked as advanced and YMMV
//
// see: two lines above, ScTimeManager, ScTime

public class ClockPatch : UE.MonoBehaviour, IPatch
{
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
        Plugin.D($"Setting clock state, paused={bPauseClock}");
        timeManager.PauseClock(bPauseClock);
    }

    private void Update()
    {
        if (ConfigManager.Instance.GetValue<BC.KeyboardShortcut>(kPauseClockActionId).IsDown())
        {
            // invert what we think the clock state currently is
            bPauseClock = !bPauseClock;
            OnTriggerUpdate();
        }
    }

    public void Initialize()
    {
        ConfigManager.Instance.StartSection("Time Management")
            .Create(kPauseClockActionId, "Pause Clock Shortcut", new BC.KeyboardShortcut(UE.KeyCode.None),
                "Pause the clock, time will not pass. Resets when you enter/leave a room.",
                null,
                new ConfigurationManagerAttributes { IsAdvanced = true }
            )
        .EndSection("Time Management");
    }

    [HL.HarmonyPatch(typeof(ScTime), nameof(ScTime.PauseClock))]
    [HL.HarmonyPrefix]
    public static void PauseClock(bool pause)
    {
        // reset the state of the clock when the system changes it
        Plugin.GetStoredPatch<ClockPatch>().bPauseClock = pause;
    }
}