using UE = UnityEngine;

namespace LKGS;

public class ClockPatch : BasePatch
{
    private BepInEx.Configuration.ConfigEntry<bool> bPauseClockEnable { get; set; }

    public override void OnTriggerUpdate()
    {
        // don't try to access something that does not exist
        var timeManager = ScGameManager.Instance?.GetTimeManager();
        if (timeManager == null) return;

        timeManager.PauseClock(bPauseClockEnable.Value);
    }

    public override void Initialize(BepInEx.Configuration.ConfigFile config)
    {
        // TODO: make this function not suck
        bPauseClockEnable = config.Bind(
            "Clock and Time Management",
            "Pause Clock",
            false,
            new BepInEx.Configuration.ConfigDescription(
                "Pause the clock completely. Time will not pass",
                null,
                new ConfigurationManagerAttributes {Order = --Config.iOrderIndex}
            )
        );
        bPauseClockEnable.SettingChanged += (_, _) => { OnTriggerUpdate(); };
    }
}