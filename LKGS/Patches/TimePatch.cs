using UE = UnityEngine;

namespace LKGS;

public class TimePatch : BasePatch
{
    private BepInEx.Configuration.ConfigEntry<bool> bClockSlowDownEnable { get; set; }
    private BepInEx.Configuration.ConfigEntry<int> iClockSlowDownMultiplier { get; set; }

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
    }

    private void SetAllPatchedValuesToDefaultValues()
    {
        // force the configs back to their default values
        fRealMinPerGameHr = fDefaultRealMinPerGameHr;
        fGameSecPerRealSec = fGameFpsTarget / fDefaultRealMinPerGameHr;
    }

    public override void OnTriggerUpdate()
    {
        // initially set the values back to default
        SetAllPatchedValuesToDefaultValues();

        // if we want to slow the clock down, apply the multiplier
        if (bClockSlowDownEnable.Value)
        {
            fRealMinPerGameHr  = fDefaultRealMinPerGameHr * iClockSlowDownMultiplier.Value;
            fGameSecPerRealSec = fGameFpsTarget / fRealMinPerGameHr;
        }

        // update the internal TimeManager with the new values
        UpdateTimeManager();
    }

    private void OnEnable()
    {
        // when we enable, let logic take over
        SetAllPatchedValuesToDefaultValues();
    }

    public override void Initialize(BepInEx.Configuration.ConfigFile config)
    {
        // TODO: make this function not suck
        int elementOrder = 100;

        bClockSlowDownEnable = config.Bind(
            "Clock and Time Management",
            "Enable Clock Slowdown",
            false,
            new BepInEx.Configuration.ConfigDescription(
                "Slow the rate at which the clock moves forward.",
                null,
                new ConfigurationManagerAttributes {Order = --elementOrder}
            )
        );
        bClockSlowDownEnable.SettingChanged += (_, _) => { OnTriggerUpdate(); };

        iClockSlowDownMultiplier = config.Bind(
            "Clock and Time Management",
            "Clock Slowdown Multiplier",
            1,
            new BepInEx.Configuration.ConfigDescription(
                "Set the time multiplier, making the day 'N' times longer.",
                new BepInEx.Configuration.AcceptableValueRange<int>(2, 8),
                new ConfigurationManagerAttributes {ShowRangeAsPercent = false, Order = --elementOrder}
            )
        );
        iClockSlowDownMultiplier.SettingChanged += (_, _) => OnTriggerUpdate();
    }
}
