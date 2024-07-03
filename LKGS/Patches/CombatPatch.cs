using System;
using System.ComponentModel;

using HL = HarmonyLib;

namespace LKGS;

// LKG has a statically set table for enemy levels on planet levels for each planet
// because of this, I do not want to modify C&K's vision for level difficulty scale
//
// when you adjust combat difficulty, a scalar value is applied to the original enemy level values
// s.t. it should feel similar to default gameplay albeit slightly adjusted
//
// see: ScPlanetEnemyGuideData, ScPlanetEnemy, ScPlanetEnemyLevel

public class CombatPatch : IPatch
{
    private enum CombatDifficulty
    {
        [Description("Default")]
        Default,
        [Description("Easy")]
        Easy,
        [Description("Peaceful")]
        Peaceful,
        [Description("Hard")]
        Hard,
    }

    private string eCombatDifficultyId = "eCombatDifficulty";
    private ScPlanetEnemyLevel CurrentSpawnCount = new();

    public void Initialize()
    {
        ConfigManager.Instance.StartSection("Character Management")
            .Create(eCombatDifficultyId, "Combat Difficulty", CombatDifficulty.Default,
                "Modifies the number of enemies that spawn.",
                null,
                new ConfigurationManagerAttributes{}
            )
        .EndSection("Character Management");
    }

    private static void ApplyNewEnemyLevelForDifficulty(CombatDifficulty combatDif, ref ScPlanetEnemyLevel input)
    {
        // do nothing for default
        if (combatDif == CombatDifficulty.Default) return;

        Plugin.D($"[ApplyNewEnemyLevelForDifficulty] - Input parameters: {input.ToDebugString()}");

        if (combatDif == CombatDifficulty.Peaceful)
        {
            // set all to zero if peaceful
            input.easyEnemies = input.hardEnemies = input.specialtyEnemies = new ScIntRange(0, 0);
            Plugin.D($"[ApplyNewEnemyLevelForDifficulty] - Peaceful mode enabled: {input.ToDebugString()}");
        }
        else if (combatDif == CombatDifficulty.Easy)
        {
            // cap the maximum allowed and lower it by a factor if easy
            // i.e. min1 = 0, max1 = ceil(max0 / 2)
            input.easyEnemies.minimumNum = input.hardEnemies.minimumNum = input.specialtyEnemies.minimumNum = 0;
            input.easyEnemies.maxiumNum = (int)Math.Ceiling(Convert.ToDouble(input.easyEnemies.maxiumNum) / 2.0);
            input.hardEnemies.maxiumNum = (int)Math.Ceiling(Convert.ToDouble(input.hardEnemies.maxiumNum) / 2.0);
            input.specialtyEnemies.maxiumNum = (int)Math.Ceiling(Convert.ToDouble(input.specialtyEnemies.maxiumNum) / 2.0);
            Plugin.D($"[ApplyNewEnemyLevelForDifficulty] - Easy mode enabled: {input.ToDebugString()}");
        }
        else if (combatDif == CombatDifficulty.Hard)
        {
            // scale the minimum value up a bit if hard
            // i.e. min1 = max0 - min0, max1 = max0
            input.easyEnemies.minimumNum = input.easyEnemies.maxiumNum - input.easyEnemies.minimumNum;
            input.hardEnemies.minimumNum = input.hardEnemies.maxiumNum - input.hardEnemies.minimumNum;
            input.specialtyEnemies.minimumNum = input.specialtyEnemies.maxiumNum - input.specialtyEnemies.minimumNum;
            Plugin.D($"[ApplyNewEnemyLevelForDifficulty] - Hard mode enabled (hell yeah): {input.ToDebugString()}");
        }
        else
        {
            Plugin.D($"Unable to determine current combat difficulty, assuming default. ({combatDif})");
        }
    }

    internal void GetEnemyLevel(ScPlanetEnemyLevel __result)
    {
        // this is called first, always

        // update the current state based on the pre-generated fields
        CurrentSpawnCount = __result;

        // modify the new state based on the currently selected difficulty
        var cbtDif = ConfigManager.Instance.GetValue<CombatDifficulty>(eCombatDifficultyId);
        ApplyNewEnemyLevelForDifficulty(cbtDif, ref CurrentSpawnCount);
    }

    internal void GetNumHardEnemies(ref int __result)
    {
        // this is called second, always
        __result = CurrentSpawnCount.hardEnemies.RandomInt;
        Plugin.D($"GetNumHardEnemies => {__result}");
    }

    internal void GetNumSpecialEnemies(ref int __result)
    {
        // this is called third, always
        __result = CurrentSpawnCount.specialtyEnemies.RandomInt;
        Plugin.D($"GetNumSpecEnemies => {__result}");
    }

    internal void GetNumEasyEnemies(ref int __result)
    {
        // this is called fourth, always
        __result = CurrentSpawnCount.easyEnemies.RandomInt;
        Plugin.D($"GetNumEasyEnemies => {__result}");
    }

    [HL.HarmonyPatch(typeof(ScPlanetEnemy), nameof(ScPlanetEnemy.GetEnemyLevel))]
    [HL.HarmonyPostfix]
    internal static void GetEnemyLevel_Postfix(ScPlanetEnemyLevel __result) // , int levelToGet)
    {
        Plugin.GetStoredPatch<CombatPatch>().GetEnemyLevel(__result);
    }

    [HL.HarmonyPatch(typeof(ScPlanetEnemyLevel), nameof(ScPlanetEnemyLevel.GetNumHardEnemies))]
    [HL.HarmonyPostfix]
    public static void GetNumHardEnemies_Postfix(ref int __result)
    {
        Plugin.GetStoredPatch<CombatPatch>().GetNumHardEnemies(ref __result);
    }

    [HL.HarmonyPatch(typeof(ScPlanetEnemyLevel), nameof(ScPlanetEnemyLevel.GetNumSpecialEnemies))]
    [HL.HarmonyPostfix]
    public static void GetNumSpecialEnemies_Postfix(ref int __result)
    {
        Plugin.GetStoredPatch<CombatPatch>().GetNumSpecialEnemies(ref __result);
    }

    [HL.HarmonyPatch(typeof(ScPlanetEnemyLevel), nameof(ScPlanetEnemyLevel.GetNumEasyEnemies))]
    [HL.HarmonyPostfix]
    public static void GetNumEasyEnemies_Postfix(ref int __result)
    {
        Plugin.GetStoredPatch<CombatPatch>().GetNumEasyEnemies(ref __result);
    }
}
