using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPage : MonoBehaviour
{
    [field:SerializeField]
    public GameObject Root {  get; private set; }
    [field:SerializeField]
    public CanvasGroup CG {  get; private set; }
    private CanvasGroupController CGcontroller { get; set; }
    public enum PageType { Config,Help,SL}
    [field:SerializeField]
    public PageType Type { get; private set; }
    private void Start()
    {
        CGcontroller ??= new(this, CG);
    }
    public virtual void Open()
    {
        CGcontroller.SetCanvasStatus(true);
        CGcontroller.Show();
    }
    public virtual void Close(bool closeAll = false)
    {
        CGcontroller.Hide();
        CGcontroller.SetCanvasStatus(false);
        if (closeAll)
        {
            MenuPanelManager.Instance.HideMenu();
        }
    }

}
