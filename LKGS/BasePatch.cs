namespace LKGS;

public abstract class BasePatch : UnityEngine.MonoBehaviour
{
    public abstract void Initialize(BepInEx.Configuration.ConfigFile config);
    public abstract void OnTriggerUpdate();
}
