using System;

namespace LKGS;

public class ConfigManager : SingletonBase<ConfigManager>
{
    private int iOrderIndex = 0;

    public int GetNextOrder() { return iOrderIndex++; }
}
