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

    private const int iCombatNumEnemiesSpawnMinDefault = 1;
    private const int iCombatNumEnemiesSpawnMaxDefault = 9;
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
            // i.e. min1 = 0, max1 = floor(max0 / 1.5)
            input.easyEnemies.minimumNum = input.hardEnemies.minimumNum = input.specialtyEnemies.minimumNum = 0;
            input.easyEnemies.maxiumNum = (int)Math.Floor(input.easyEnemies.maxiumNum / 1.5);
            input.hardEnemies.maxiumNum = (int)Math.Floor(input.hardEnemies.maxiumNum / 1.5);
            input.specialtyEnemies.maxiumNum = (int)Math.Floor(input.specialtyEnemies.maxiumNum / 1.5);
            Plugin.D($"[ApplyNewEnemyLevelForDifficulty] - Easy mode enabled: {input.ToDebugString()}");
        }
        else if (combatDif == CombatDifficulty.Hard)
        {
            // force the minimum to 1, if 0
            input.easyEnemies.minimumNum = input.easyEnemies.minimumNum == 0 ? 1 : input.easyEnemies.minimumNum;
            input.hardEnemies.minimumNum = input.hardEnemies.minimumNum == 0 ? 1 : input.hardEnemies.minimumNum;
            input.specialtyEnemies.minimumNum = input.specialtyEnemies.minimumNum == 0 ? 1 : input.specialtyEnemies.minimumNum;

            // if the maximum is 0, bring it up
            input.easyEnemies.maxiumNum = input.easyEnemies.maxiumNum == 0 ? 2 : input.easyEnemies.maxiumNum;
            input.hardEnemies.maxiumNum = input.hardEnemies.maxiumNum == 0 ? 2 : input.hardEnemies.maxiumNum;
            input.specialtyEnemies.maxiumNum = input.specialtyEnemies.maxiumNum == 0 ? 2 : input.specialtyEnemies.maxiumNum;

            // scale the maximum value up a bit if hard
            // i.e. min1 = min0 + ceil(min0 / 2), max1 = max0 + ceil(min0 / 2)
            input.easyEnemies.maxiumNum = input.easyEnemies.maxiumNum + (int)Math.Ceiling(input.easyEnemies.minimumNum / 2.0);
            input.hardEnemies.maxiumNum = input.hardEnemies.maxiumNum + (int)Math.Ceiling(input.hardEnemies.minimumNum / 2.0);
            input.specialtyEnemies.maxiumNum = input.specialtyEnemies.maxiumNum + (int)Math.Ceiling(input.specialtyEnemies.minimumNum / 2.0);
            input.easyEnemies.minimumNum = input.easyEnemies.minimumNum + (int)Math.Ceiling(input.easyEnemies.minimumNum / 2.0);
            input.hardEnemies.minimumNum = input.hardEnemies.minimumNum + (int)Math.Ceiling(input.hardEnemies.minimumNum / 2.0);
            input.specialtyEnemies.minimumNum = input.specialtyEnemies.minimumNum + (int)Math.Ceiling(input.specialtyEnemies.minimumNum / 2.0);

            // don't let the values be out right
            input.Clamp(iCombatNumEnemiesSpawnMinDefault, iCombatNumEnemiesSpawnMaxDefault);

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
        // how do csharp refs even work like what benefit does reflection
        // and garbage collection really provide I want to go back to c++
        CurrentSpawnCount.CopyFrom(__result);

        // modify the new state based on the currently selected difficulty
        var cbtDif = ConfigManager.Instance.GetValue<CombatDifficulty>(eCombatDifficultyId);
        ApplyNewEnemyLevelForDifficulty(cbtDif, ref CurrentSpawnCount);
    }

    internal void GetNumHardEnemies(ref int __result)
    {
        // this is called second, always
        int before = __result;
        __result = CurrentSpawnCount.hardEnemies.RandomInt;
        Plugin.D($"GetNumHardEnemies | ({CurrentSpawnCount.levelInt}) {before} => {__result}");
    }

    internal void GetNumSpecialEnemies(ref int __result)
    {
        // this is called third, always
        int before = __result;
        __result = CurrentSpawnCount.specialtyEnemies.RandomInt;
        Plugin.D($"GetNumSpecEnemies | ({CurrentSpawnCount.levelInt}) {before} => {__result}");
    }

    internal void GetNumEasyEnemies(ref int __result)
    {
        // this is called fourth, always
        int before = __result;
        __result = CurrentSpawnCount.easyEnemies.RandomInt;
        Plugin.D($"GetNumEasyEnemies | ({CurrentSpawnCount.levelInt}) {before} => {__result}");
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
