using System;
using System.Runtime.CompilerServices;

namespace LKGS;

public static class ScAPIExtensions
{
    public static void CopyFrom(this ScPlanetEnemyLevel lhs, ScPlanetEnemyLevel rhs)
    {
        lhs.levelInt = rhs.levelInt;
        lhs.easyEnemies = new ScIntRange(rhs.easyEnemies.minimumNum, rhs.easyEnemies.maxiumNum);
        lhs.hardEnemies = new ScIntRange(rhs.hardEnemies.minimumNum, rhs.hardEnemies.maxiumNum);
        lhs.specialtyEnemies = new ScIntRange(rhs.specialtyEnemies.minimumNum, rhs.specialtyEnemies.maxiumNum);
    }

    public static void Clamp(this ScPlanetEnemyLevel lhs, int min, int max)
    {
        lhs.easyEnemies.minimumNum = Math.Clamp(lhs.easyEnemies.minimumNum, min, max);
        lhs.easyEnemies.maxiumNum = Math.Clamp(lhs.easyEnemies.maxiumNum, min, max);
        lhs.hardEnemies.minimumNum = Math.Clamp(lhs.hardEnemies.minimumNum, min, max);
        lhs.hardEnemies.maxiumNum = Math.Clamp(lhs.hardEnemies.maxiumNum, min, max);
        lhs.specialtyEnemies.minimumNum = Math.Clamp(lhs.specialtyEnemies.minimumNum, min, max);
        lhs.specialtyEnemies.maxiumNum = Math.Clamp(lhs.specialtyEnemies.maxiumNum, min, max);
    }
}