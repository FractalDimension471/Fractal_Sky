using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ڼ��Э�̣���װ��
/// </summary>
public class CoroutineWrapper
{
    public CoroutineWrapper(MonoBehaviour owner,Coroutine co)
    {
        this.owner = owner;
        this.co = co;
    }
    #region ����
    private readonly MonoBehaviour owner;
    private readonly Coroutine co;
    public bool isDone = false;
    #endregion
    #region ����
    public void Stop()
    {
        owner.StopCoroutine(co);
        isDone = true;
    }
    #endregion
}
