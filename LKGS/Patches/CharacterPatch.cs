using BepInEx.Configuration;

namespace LKGS;

public class CharacterPatch : BasePatch
{
    public CharacterPatch(ConfigManager configManager) : base(configManager) {}

    protected override void Initialize()
    {
        // throw new System.NotImplementedException();
    }

    protected override void OnTriggerUpdate()
    {
        // throw new System.NotImplementedException();
    }

    [HarmonyLib.HarmonyPatch(typeof(ScPlayerStats), nameof(ScPlayerStats.ExhaustedPenalty))]
    [HarmonyLib.HarmonyPrefix]
    public static bool ExhaustedPenalty()
    {
        // don't let the exhausted penalty be set
        return false;
    }

    [HarmonyLib.HarmonyPatch(typeof(ScPlayerStats), nameof(ScPlayerStats.LateNightPenalty))]
    [HarmonyLib.HarmonyPrefix]
    public static bool LateNightPenalty()
    {
        // don't let the late night penalty be set
        return false;
    }

    [HarmonyLib.HarmonyPatch(typeof(ScPlayerStats), nameof(ScPlayerStats.AdjustStat))]
    [HarmonyLib.HarmonyPrefix]
    public static void AdjustStat(PlayerStat statToAdjust, ref float amount)
    {
        if (statToAdjust == PlayerStat.Energy) amount = 0.0f; // inf stamina
        if (statToAdjust == PlayerStat.Health) amount = 0.0f; // god mode
    }
}
