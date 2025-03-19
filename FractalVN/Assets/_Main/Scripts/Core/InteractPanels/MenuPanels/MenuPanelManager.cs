using System.Linq;
using UnityEngine;

public class MenuPanelManager : MonoBehaviour
{
    public static MenuPanelManager Instance { get; private set; }
    [field: SerializeField]
    public CanvasGroup RootCanvas { get; private set; }
    private CanvasGroupController CGcontroller { get; set; }
    [field: SerializeField]
    public MenuPage[] Pages { get; private set; }
    private MenuPage ActivePage { get; set; }
    private bool IsMenuOpen { get; set; } = false;
    private UIConfirmationPage ConfirmationPage => UIConfirmationPage.Instance;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        CGcontroller = new(this, RootCanvas);
    }
    private MenuPage GetPage(MenuPage.PageType type)
    {
        return Pages.FirstOrDefault(p => p.Type == type);//返回第一个匹配值
    }
    private void OpenPage(MenuPage page)
    {
        if (!IsMenuOpen)
        {
            ShowMenu();
        }

        if (page == null)
        {
            return;
        }
        if (ActivePage != null && ActivePage != page)
        {
            ActivePage.Close();
        }
        page.Open();
        ActivePage = page;
    }

    public void OpenSavePage()
    {
        var page = GetPage(MenuPage.PageType.SL);
        var currentPage = page.Root.GetComponent<SLPage>();
        currentPage.CurrentFuction = SLPage.MenuFuction.Save;
        OpenPage(page);
    }
    public void OpenLoadPage()
    {
        var page = GetPage(MenuPage.PageType.SL);
        var currentPage = page.Root.GetComponent<SLPage>();
        currentPage.CurrentFuction = SLPage.MenuFuction.Load;
        OpenPage(page);
    }
    public void OpenConfigPage()
    {
        var page = GetPage(MenuPage.PageType.Config);
        OpenPage(page);
    }
    public void OpenHelpPage()
    {
        var page = GetPage(MenuPage.PageType.Help);
        OpenPage(page);
    }
    public void OpenGalleryPage()
    {
        var page = GetPage(MenuPage.PageType.Gallery);
        var currentPage = page.Root.GetComponent<GalleryMenu>();
        currentPage.Initialize();
        OpenPage(page);

    }
    public void ShowMenu()
    {
        IsMenuOpen = true;
        CGcontroller.SetCanvasStatus(true);
        CGcontroller.Show();
    }
    public void HideMenu()
    {
        IsMenuOpen = false;
        CGcontroller.Hide();
        CGcontroller.SetCanvasStatus(false);
    }
    public void BackToTitle()
    {
        ConfirmationPage.Show("Back to main menu?", new UIConfirmationPage.ConfirmationButton("Yes", BackingToTitle), new UIConfirmationPage.ConfirmationButton("No", null));
    }
    private void BackingToTitle()
    {
        ConfigData.ActiveConfig.Save();
        AudioManager.Instance.CachedSounds.Clear();
        AudioManager.Instance.StopAll();
        UnityEngine.SceneManagement.SceneManager.LoadScene(MainMenu.ID_StartScene);
    }
    public void QuitGame()
    {
        ConfirmationPage.Show("Quit the game?", new UIConfirmationPage.ConfirmationButton("Yes", QuittingGame), new UIConfirmationPage.ConfirmationButton("No", null));
    }
    private void QuittingGame()
    {
        Application.Quit();
    }
}
