namespace LKGS;

public static class ToStringUtilities
{
    public static string ToDebugString(this ScPlanetEnemyLevel input)
    {
        return $"level: {input.levelInt}"
             + $", easy: {input.easyEnemies.minimumNum} {input.easyEnemies.maxiumNum}"
             + $", hard: {input.hardEnemies.minimumNum} {input.hardEnemies.maxiumNum}"
             + $", spec: {input.specialtyEnemies.minimumNum} {input.specialtyEnemies.maxiumNum}";
    }
}
