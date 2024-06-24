using HL = HarmonyLib;
namespace LKGS;

public class TimePatch : BasePatch
{
    private string bClockSlowDownEnableId = "bClockSlowDownEnable";
    private string iClockSlowDownMultiplier = "iClockSlowDownMultiplier";

    private const float fGameFpsTarget = 60.0f;
    private const float fDefaultRealMinPerGameHr = 0.65f;

    private float fRealMinPerGameHr;
    private float fGameSecPerRealSec;

    public TimePatch(ConfigManager configManager) : base(configManager) {}

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

    protected override void OnTriggerUpdate()
    {
        // initially set the values back to default
        SetAllPatchedValuesToDefaultValues();

        // if we want to slow the clock down, apply the multiplier
        if (ConfigManager.Get<bool>(bClockSlowDownEnableId))
        {
            fRealMinPerGameHr  = fDefaultRealMinPerGameHr * ConfigManager.Get<int>(iClockSlowDownMultiplier);
            fGameSecPerRealSec = fGameFpsTarget / fRealMinPerGameHr;
        }

        // update the internal TimeManager with the new values
        UpdateTimeManager();
    }

    protected override void Initialize()
    {
        SetAllPatchedValuesToDefaultValues();

        ConfigManager
            .StartSection("Time Management")
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
        Plugin.GetStoredPatch<TimePatch>().OnTriggerUpdate();
    }
}
