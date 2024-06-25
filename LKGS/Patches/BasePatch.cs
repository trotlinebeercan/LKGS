namespace LKGS;

public interface BasePatch
{
    public abstract void Initialize();
    public virtual void OnTriggerUpdate() {}
    public virtual void OnActiveSceneChanged() {}
}
