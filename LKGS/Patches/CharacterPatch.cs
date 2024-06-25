using HL = HarmonyLib;

namespace LKGS;

public class CharacterPatch : IPatch
{
    private string bEnableInfiniteEnergyId = "bEnableInfiniteEnergy";
    private string bEnableInfiniteHealthId = "bEnableInfiniteHealth";
    private string bPreventNextDayPenaltiesId = "bPreventNextDayPenalties";
    private string bEnableDoubleMovementSpeedId = "bEnableDoubleMovementSpeed";
    private string bEnableFullyChargeToolsId = "bEnableFullyChargedTools";

    // defaults: walk=2 run=5.5 planet=5.3
    private const float fDefaultRunSpeed = 5.5f;

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
            .Create(bEnableDoubleMovementSpeedId, "Double Movement Speed", false,
                "Increases the speed of all forms of movement by 2x.",
                null,
                new ConfigurationManagerAttributes{}
            )
            .Create(bEnableFullyChargeToolsId, "Tools Always Fully Charged", false,
                "Makes all tools act as if they were fully charged, all the time.",
                null,
                new ConfigurationManagerAttributes{}
            )
        .EndSection("Character Management");
    }

    private bool PreventPenalties()
    {
        return ConfigManager.Instance.GetValue<bool>(bPreventNextDayPenaltiesId);
    }

    private void AdjustStatImpl(PlayerStat statToAdjust, ref float amount)
    {
        // should be simple - just set the incoming decrement amount to 0 for no-op
        if (statToAdjust == PlayerStat.Energy)
            amount = ConfigManager.Instance.GetValue<bool>(bEnableInfiniteEnergyId) ? 0.0f : amount;
        if (statToAdjust == PlayerStat.Health)
            amount = ConfigManager.Instance.GetValue<bool>(bEnableInfiniteHealthId) ? 0.0f : amount;
    }

    private void AdjustMovementSpeed()
    {
        var pc = ScGameManager.Instance?.GetPlayerController();
        if (pc != null)
        {
            bool enabled = ConfigManager.Instance.GetValue<bool>(bEnableDoubleMovementSpeedId);
            if (enabled)
            {
                pc.speed = fDefaultRunSpeed * 2f;
            }
        }
    }

    private void SetChargeDurationToNothing()
    {
        var pc = ScGameManager.Instance?.GetPlayerController();
        if (pc != null)
        {
            if (ConfigManager.Instance.GetValue<bool>(bEnableFullyChargeToolsId))
            {
                pc.secondsToCharge = 0.03f;
            }
        }
    }

    [HL.HarmonyPatch(typeof(ScPlayerStats), nameof(ScPlayerStats.ExhaustedPenalty))]
    [HL.HarmonyPrefix]
    public static bool ExhaustedPenalty()
    {
        // don't let the exhausted penalty be set if we're Preventing penalties
        return !Plugin.GetStoredPatch<CharacterPatch>().PreventPenalties();
    }

    [HL.HarmonyPatch(typeof(ScPlayerStats), nameof(ScPlayerStats.LateNightPenalty))]
    [HL.HarmonyPrefix]
    public static bool LateNightPenalty()
    {
        // don't let the late night penalty be set if we're Preventing penalties
        return !Plugin.GetStoredPatch<CharacterPatch>().PreventPenalties();
    }

    [HL.HarmonyPatch(typeof(ScPlayerStats), nameof(ScPlayerStats.AdjustStat))]
    [HL.HarmonyPrefix]
    public static void AdjustStat(PlayerStat statToAdjust, ref float amount)
    {
        Plugin.GetStoredPatch<CharacterPatch>().AdjustStatImpl(statToAdjust, ref amount);
    }

    [HL.HarmonyPatch(typeof(ScCharacter), nameof(ScCharacter.Move))]
    [HL.HarmonyPrefix]
    public static void Move(ScCharacter __instance)
    {
        // we only want this to happen for the player, not all NPCs
        if (__instance == ScGameManager.Instance?.GetPlayerController())
        {
            Plugin.GetStoredPatch<CharacterPatch>().AdjustMovementSpeed();
        }
    }

    [HL.HarmonyPatch(typeof(ScPlayerController), nameof(ScPlayerController.SetChargeDuration))]
    [HL.HarmonyPostfix]
    public static void SetChargeDuration(ScPlayerController __instance)
    {
        Plugin.GetStoredPatch<CharacterPatch>().SetChargeDurationToNothing();
    }
}
