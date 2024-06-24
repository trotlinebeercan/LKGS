namespace LKGS;

public abstract class BasePatch
{
    protected ConfigManager ConfigManager;
    public BasePatch(ConfigManager configManager)
    {
        ConfigManager = configManager;
        Initialize();
    }

    protected abstract void Initialize();
    protected abstract void OnTriggerUpdate();
}
