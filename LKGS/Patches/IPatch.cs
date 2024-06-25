namespace LKGS;

public interface IPatch
{
    public abstract void Initialize();
    public virtual void OnTriggerUpdate() {}
    public virtual void OnActiveSceneChanged() {}
}
