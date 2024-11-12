using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于监管协程（封装）
/// </summary>
public class CoroutineWrapper
{
    public CoroutineWrapper(MonoBehaviour owner,Coroutine co)
    {
        this.owner = owner;
        this.co = co;
    }
    #region 属性
    private readonly MonoBehaviour owner;
    private readonly Coroutine co;
    public bool isDone = false;
    #endregion
    #region 方法
    public void Stop()
    {
        owner.StopCoroutine(co);
        isDone = true;
    }
    #endregion
}
