using HL = HarmonyLib;
using BC = BepInEx.Configuration;
using UE = UnityEngine;

namespace LKGS;

// NOTE: this does not work as expected... weird things happen
//       do not try this at home (or do, idgaf)

public class ClockPatch : BasePatch
{
    private BC.ConfigEntry<BC.KeyboardShortcut> bPauseClockToggle { get; set; }
    private bool bPauseClock = false;

    public override void OnTriggerUpdate()
    {
        // don't try to access something that does not exist
        var timeManager = ScGameManager.Instance?.GetTimeManager();
        if (timeManager == null) return;

        // don't try to modify a setting that doesn't need to change
        if (timeManager.pauseClock == bPauseClock) return;

        // do the thing
        timeManager.PauseClock(bPauseClock);
    }

    private void Update()
    {
        if (bPauseClockToggle.Value.IsDown())
        {
            bPauseClock = !bPauseClock;
            OnTriggerUpdate();
        }
    }

    public override void Initialize(BC.ConfigFile config)
    {
        // TODO: make this function not suck
        bPauseClockToggle = config.Bind(
            "Clock and Time Management",
            "Pause Clock",
            new BC.KeyboardShortcut(UE.KeyCode.F2),
            new BC.ConfigDescription(
                "Pause the clock completely. Time will not pass. Resets when you enter/leave a room.",
                null,
                new ConfigurationManagerAttributes {Order = --Config.iOrderIndex}
            )
        );
    }

    //[HL.HarmonyPatch(typeof(ScTime), nameof(ScTime.PauseClock))]
    //[HL.HarmonyPostfix]
    public static void PauseClock()
    {
        Plugin.GetStoredPatch<ClockPatch>().OnTriggerUpdate();
    }
}