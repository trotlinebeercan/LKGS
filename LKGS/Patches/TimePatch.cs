using HL = HarmonyLib;

namespace LKGS;

// the game is capped at 60fps, so each frame update sets the "real minutes per in-game hour"
// to be roughly 0.65, or 39sec per in-game hour, or 741 sec / 12.35min ber in-game hour.
// each tick checks this delta time and increments the in-game clock by 15min intervals
// using this precomputed ratio
//
// to adjust clock speed, we simply adjust the precomputed ratio via scalars.
// want to make the game twice as slow? multiply the ratio by 2
//
// see: ScTimeManager, ScTime

public class TimePatch : IPatch
{
    private string bClockSlowDownEnableId = "bClockSlowDownEnable";
    private string iClockSlowDownMultiplier = "iClockSlowDownMultiplier";

    private const float fGameFpsTarget = 60.0f;
    private const float fDefaultRealMinPerGameHr = 0.65f;

    private float fRealMinPerGameHr;
    private float fGameSecPerRealSec;

    private void UpdateTimeManager()
    {
        // don't try to access something that does not exist
        var timeManager = ScGameManager.Instance?.GetTimeManager();
        if (timeManager == null) return;

        timeManager.realMinPerGameHr.floatValue = fRealMinPerGameHr;
        timeManager.gameSecPerRealSec = fGameSecPerRealSec;

        Plugin.D($"New values: m/gh={timeManager.realMinPerGameHr.floatValue}, gs/s={timeManager.gameSecPerRealSec}");
    }

    private void SetAllPatchedValuesToDefaultValues()
    {
        // force the configs back to their default values
        fRealMinPerGameHr = fDefaultRealMinPerGameHr;
        fGameSecPerRealSec = fGameFpsTarget / fDefaultRealMinPerGameHr;
    }

    public void OnTriggerUpdate()
    {
        // initially set the values back to default
        SetAllPatchedValuesToDefaultValues();

        // if we want to slow the clock down, apply the multiplier
        if (ConfigManager.Instance.GetValue<bool>(bClockSlowDownEnableId))
        {
            fRealMinPerGameHr  = fDefaultRealMinPerGameHr * ConfigManager.Instance.GetValue<int>(iClockSlowDownMultiplier);
            fGameSecPerRealSec = fGameFpsTarget / fRealMinPerGameHr;
        }

        // update the internal TimeManager with the new values
        UpdateTimeManager();
    }

    public void Initialize()
    {
        SetAllPatchedValuesToDefaultValues();

        ConfigManager.Instance.StartSection("Time Management")
            .Create(bClockSlowDownEnableId, "Enable Clock Slowdown", false,
                "Slow the rate at which the clock moves forward.",
                null,
                new ConfigurationManagerAttributes {},
                (_, _) => { OnTriggerUpdate(); }
            )
            .Create(iClockSlowDownMultiplier, "Clock Speed Multiplier", 1,
                "Set the time multiplier, making the day 'N' times longer.",
                new BepInEx.Configuration.AcceptableValueRange<int>(2, 8),
                new ConfigurationManagerAttributes {ShowRangeAsPercent = false},
                (_, _) => { OnTriggerUpdate(); }
            )
        .EndSection("Time Management");
    }

    [HL.HarmonyPatch(typeof(ScTime), nameof(ScTime.SetUpManager))]
    [HL.HarmonyPrefix]
    public static void SetUpManager()
    {
        // this prefix ensures the variables will always have our values
        // if something else modifies them after we do
        Plugin.GetStoredPatch<TimePatch>().OnTriggerUpdate();
    }
}
