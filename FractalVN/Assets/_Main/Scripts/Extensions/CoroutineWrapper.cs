using UnityEngine;

/// <summary>
/// 用于监管协程（封装）
/// </summary>
public class CoroutineWrapper
{
    public CoroutineWrapper(MonoBehaviour owner, Coroutine co)
    {
        Owner = owner;
        CommandCoroutine = co;
    }
    #region 属性
    private MonoBehaviour Owner { get; }
    private Coroutine CommandCoroutine { get; }
    public bool IsDone { get; internal set; } = false;
    #endregion
    #region 方法
    public void Stop()
    {
        Owner.StopCoroutine(CommandCoroutine);
        IsDone = true;
    }
    #endregion
}
