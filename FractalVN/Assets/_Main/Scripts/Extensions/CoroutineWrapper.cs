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
        Owner = owner;
        CommandCoroutine = co;
    }
    #region ����
    private MonoBehaviour Owner { get; }
    private Coroutine CommandCoroutine { get; }
    public bool IsDone { get; internal set; } = false;
    #endregion
    #region ����
    public void Stop()
    {
        Owner.StopCoroutine(CommandCoroutine);
        IsDone = true;
    }
    #endregion
}
