namespace LKGS;

public abstract class BasePatch : UnityEngine.MonoBehaviour
{
    public abstract void Initialize(BepInEx.Configuration.ConfigFile config);
}
