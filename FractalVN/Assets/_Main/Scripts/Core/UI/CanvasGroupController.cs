using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;
using static UnityEngine.Rendering.DebugUI;

public class CanvasGroupController
{
    #region ÊôÐÔ/Property
    private MonoBehaviour Owner { get; set; }
    private CanvasGroup RootCG {  get; set; }
    private Coroutine Co_showing {  get; set; }
    private Coroutine Co_hiding { get; set; }

    public bool IsShowing => Co_showing != null;
    public bool IsHiding => Co_hiding != null;
    public bool IsFading => IsShowing || IsHiding;
    public bool IsVisible => Co_showing != null || RootCG.alpha > 0;
    public float Alpha { get { return RootCG.alpha; } set { RootCG.alpha = value; } }
    #endregion

    #region ·½·¨/Method
    public CanvasGroupController(MonoBehaviour owner, CanvasGroup rootCG)
    {
        this.Owner = owner;
        this.RootCG = rootCG;
    }
    public Coroutine Show(float speedMultiplier = 1f, bool immediate = false)
    {
        if (IsShowing)
        {
            return null;
        }
        if (IsHiding)
        {
            Owner.StopCoroutine(Co_hiding);
            Co_hiding = null;
        }
        Co_showing = Owner.StartCoroutine(Fading(1, speedMultiplier, immediate));
        return Co_showing;
    }
    public Coroutine Hide(float speedMultiplier = 1f, bool immediate = false)
    {
        if (IsHiding)
        {
            return null;
        }
        if (IsShowing)
        {
            Owner.StopCoroutine(Co_showing);
            Co_showing = null;
        }
        Co_hiding = Owner.StartCoroutine(Fading(0, speedMultiplier, immediate));
        return Co_hiding;
    }
    private IEnumerator Fading(float targetAlpha, float speedMultiplier, bool immediate)
    {
        CanvasGroup cg = RootCG;
        if (immediate)
        {
            cg.alpha = targetAlpha;
        }
        while (cg.alpha != targetAlpha)
        {
            if (DialogueSystem.Instance != null)
            {
                cg.alpha = Mathf.MoveTowards(cg.alpha, targetAlpha, Time.deltaTime * DialogueSystem.Instance.TransitionSpeed * speedMultiplier);
            }
            else
            {
                cg.alpha = Mathf.MoveTowards(cg.alpha, targetAlpha, Time.deltaTime * 4.7f * speedMultiplier);
            }
            yield return null;
        }
        Co_hiding = null;
        Co_showing = null;
    }
    public void SetCanvasStatus(bool active)
    {
        RootCG.interactable = active;
        RootCG.blocksRaycasts = active;
    }
    #endregion

}
