using UnityEngine;

public class MenuPage : MonoBehaviour
{
    [field: SerializeField]
    public GameObject Root { get; private set; }
    [field: SerializeField]
    public CanvasGroup CG { get; private set; }
    public CanvasGroupController CGcontroller { get; internal set; }
    public enum PageType { Config, Help, SL, Gallery }
    [field: SerializeField]
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
