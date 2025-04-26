using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FunctionPanel : MonoBehaviour
{
    public static FunctionPanel Instance { get; private set; }
    [field: SerializeField]
    public CanvasGroup CG { get; internal set; }
    [field: SerializeField]
    public CanvasGroup ButtonsCG { get; internal set; }
    [field: SerializeField]
    public GameObject HelpPage { get; internal set; }
    [field: SerializeField]
    public GameObject SettingPage { get; internal set; }
    private CanvasGroupController CGController { get; set; }
    public enum PageType { Help, Setting }
    public PageType ActiveType { get; internal set; }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        CGController = new(this, CG);
    }
    public void Show(PageType page)
    {
        CGController.SetCanvasStatus(true);
        CGController.Show();
        switch (page)
        {
            case PageType.Help:
                HelpPage.SetActive(true);
                SettingPage.SetActive(false);
                break;
            case PageType.Setting:
                HelpPage.SetActive(false);
                SettingPage.SetActive(true);
                break;
        }
    }
    public void Hide()
    {
        CGController.Hide();
        CGController.SetCanvasStatus(false);
    }
}
