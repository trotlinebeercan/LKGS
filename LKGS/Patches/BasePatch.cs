namespace LKGS;

public abstract class BasePatch : UnityEngine.MonoBehaviour
{
    public abstract void Initialize();
    public virtual void OnTriggerUpdate() {}
    public virtual void OnActiveSceneChanged() {}
}
