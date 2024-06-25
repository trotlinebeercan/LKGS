using BepInEx.Configuration;

namespace LKGS;

public class CharacterPatch : IPatch
{
    private string bEnableInfiniteEnergyId = "bEnableInfiniteEnergy";
    private string bEnableInfiniteHealthId = "bEnableInfiniteHealth";
    private string bPreventNextDayPenaltiesId = "bPreventNextDayPenalties";

    public void Initialize()
    {
        ConfigManager.Instance.StartSection("Character Management")
            .Create(bEnableInfiniteEnergyId, "Infinite Energy", false,
                "Prevents Energy depletion when using any tool.",
                null,
                new ConfigurationManagerAttributes {}
            )
            .Create(bEnableInfiniteHealthId, "Infinite Health", false,
                "Prevents Health depletion when taking any damage.",
                null,
                new ConfigurationManagerAttributes {}
            )
            .Create(bPreventNextDayPenaltiesId, "Prevent Next Day Penalties", false,
                "Staying up too late or depleting all Energy will cause a penalty on the next day.",
                null,
                new ConfigurationManagerAttributes {}
            )
        .EndSection("Character Management");
    }

    private bool PreventPenalties()
    {
        return ConfigManager.Instance.GetValue<bool>(bPreventNextDayPenaltiesId);
    }

    private void AdjustStatImpl(PlayerStat statToAdjust, ref float amount)
    {
        if (statToAdjust == PlayerStat.Energy)
            amount = ConfigManager.Instance.GetValue<bool>(bEnableInfiniteEnergyId) ? 0.0f : amount;
        if (statToAdjust == PlayerStat.Health)
            amount = ConfigManager.Instance.GetValue<bool>(bEnableInfiniteHealthId) ? 0.0f : amount;
    }

    [HarmonyLib.HarmonyPatch(typeof(ScPlayerStats), nameof(ScPlayerStats.ExhaustedPenalty))]
    [HarmonyLib.HarmonyPrefix]
    public static bool ExhaustedPenalty()
    {
        // don't let the exhausted penalty be set if we're Preventing penalties
        return !Plugin.GetStoredPatch<CharacterPatch>().PreventPenalties();
    }

    [HarmonyLib.HarmonyPatch(typeof(ScPlayerStats), nameof(ScPlayerStats.LateNightPenalty))]
    [HarmonyLib.HarmonyPrefix]
    public static bool LateNightPenalty()
    {
        // don't let the late night penalty be set if we're Preventing penalties
        return !Plugin.GetStoredPatch<CharacterPatch>().PreventPenalties();
    }

    [HarmonyLib.HarmonyPatch(typeof(ScPlayerStats), nameof(ScPlayerStats.AdjustStat))]
    [HarmonyLib.HarmonyPrefix]
    public static void AdjustStat(PlayerStat statToAdjust, ref float amount)
    {
        Plugin.GetStoredPatch<CharacterPatch>().AdjustStatImpl(statToAdjust, ref amount);
    }
}
