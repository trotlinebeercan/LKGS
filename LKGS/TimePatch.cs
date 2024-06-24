using UE = UnityEngine;

namespace LKGS;
public class TimePatch : BasePatch
{
    private BepInEx.Configuration.ConfigEntry<bool> bPauseClockEnable { get; set; }
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

    private void OnTriggerUpdate()
    {
        // initially set the values back to default
        SetAllPatchedValuesToDefaultValues();

        // if the clock is paused, set the values to something very close to 0 and do nothing else
        if (bPauseClockEnable.Value)
        {
            fRealMinPerGameHr = fGameSecPerRealSec = 0.0001f;
        }
        // else if we want to slow the clock down, apply the multiplier
        else if (bClockSlowDownEnable.Value)
        {
            fRealMinPerGameHr  = fDefaultRealMinPerGameHr * iClockSlowDownMultiplier.Value;
            fGameSecPerRealSec = fGameFpsTarget / fRealMinPerGameHr;
        }

        // update the internal TimeManager with the new values
        UpdateTimeManager();
    }

    private void OnDisable()
    {
        // when we disable, just reset back to default
        SetAllPatchedValuesToDefaultValues();
        // don't trigger logic, just update with defaults
        UpdateTimeManager();
    }

    private void Awake()
    {
        // when we enable, let logic take over
        SetAllPatchedValuesToDefaultValues();
        OnTriggerUpdate();
    }

    public override void Initialize(BepInEx.Configuration.ConfigFile config)
    {
        int elementOrder = 100;

        bPauseClockEnable = config.Bind(
            "ooooo Clock and Time Management ooooo",
            "Pause Clock",
            false,
            new BepInEx.Configuration.ConfigDescription(
                "Pause the clock completely. Time will not pass",
                null,
                new ConfigurationManagerAttributes {Order = --elementOrder}
            )
        );
        bPauseClockEnable.SettingChanged += (_, _) => { OnTriggerUpdate(); NotifyUserOfChangesToClock(); };

        bClockSlowDownEnable = config.Bind(
            "ooooo Clock and Time Management ooooo",
            "Enable Clock Slowdown",
            false,
            new BepInEx.Configuration.ConfigDescription(
                "Slow the rate at which the clock moves forward.",
                null,
                new ConfigurationManagerAttributes {Order = --elementOrder}
            )
        );
        bClockSlowDownEnable.SettingChanged += (_, _) => { OnTriggerUpdate(); NotifyUserOfChangesToClock(); };

        iClockSlowDownMultiplier = config.Bind(
            "ooooo Clock and Time Management ooooo",
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

    private void NotifyUserOfChangesToClock() // TODO: move this out of here and into some UI/Notif manager
    {
        var notifManager = ScGameManager.Instance?.notificationManager;
        if (notifManager == null) return;

        string message = bPauseClockEnable.Value ? "Paused" : bClockSlowDownEnable.Value ? "Slowed" : "Running";
        ScTextSet textSet = new ScTextSet
        {
            BoxType = BoxType.Notification,
            Text = $"Hello Amelia! Clock is {message}"
        };
        ScGeneralText clockStatusChanged = new ScGeneralText("key", true, new ScTextSet[]{textSet});
        notifManager.AddNotificationToQueue(clockStatusChanged, notifManager.spacePackIcon, true);
    }
}
