namespace LKGS;

public class ScIntRangeExt
{
    public ScIntRange ScIntRange { get; private set; }
    public ScIntRangeExt(ScIntRange input) => ScIntRange = input;
    public static implicit operator ScIntRangeExt(ScIntRange lhs) => new ScIntRangeExt(lhs);

    public static ScIntRangeExt Zero => new ScIntRange(0, 0);

    public static ScIntRangeExt operator/(ScIntRangeExt lhs, int scalar)
    {
        return new ScIntRange(lhs.ScIntRange.minimumNum / scalar, lhs.ScIntRange.maxiumNum);
    }

    public static ScIntRangeExt operator*(ScIntRangeExt lhs, int scalar)
    {
        return new ScIntRange(lhs.ScIntRange.minimumNum * scalar, lhs.ScIntRange.maxiumNum);
    }

    public static ScIntRangeExt operator+(ScIntRangeExt lhs, int scalar)
    {
        return new ScIntRange(lhs.ScIntRange.minimumNum + scalar, lhs.ScIntRange.maxiumNum);
    }
}
