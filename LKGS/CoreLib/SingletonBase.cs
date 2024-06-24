namespace LKGS;

public abstract class SingletonBase<T> where T : SingletonBase<T>, new()
{
    private static readonly System.Lazy<T> _arbiter = new System.Lazy<T>(() => new T());
    public static T Instance => _arbiter.Value;
}
