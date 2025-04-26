using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainMenuPanel : MonoBehaviour
{
    public static MainMenuPanel Instance { get; private set; }
    [field:SerializeField]
    public CanvasGroup RootCG { get; internal set; }
    [field: SerializeField]
    public CanvasGroup HelpPageCG { get; internal set; }
    private CanvasGroupController HelpPageCGC { get; set; }
    private CanvasGroupController RootCGC { get; set; }
    private event Func<IEnumerator> StartNewGame;
    private event Action QuitGame;
    private event Action OpenHelpPage;
    private event Action CloseHelpPage;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        StartNewGame += StartingNewGame;
        QuitGame += QuittingGame;
        OpenHelpPage += OpeningHelpPage;
        CloseHelpPage += ClosingHelpPage;

        HelpPageCGC = new(this, HelpPageCG);
        RootCGC = new(this, RootCG);
    }
    public void OnStartGameButtonClicked()
    {
        StartCoroutine(StartNewGame?.Invoke());
    }
    public void OnQuitGameButtonClicked()
    {
        QuitGame?.Invoke();
    }
    public void OnOpenHelpPageButtonClicked()
    {
        OpenHelpPage?.Invoke();
    }
    public void OnCloseHelpPageButtonClicked()
    {
        CloseHelpPage?.Invoke();
    }
    private void OpeningHelpPage()
    {
        HelpPageCGC.SetCanvasStatus(true);
        HelpPageCGC.Show();
    }
    private void ClosingHelpPage()
    {
        HelpPageCGC.Hide();
        HelpPageCGC.SetCanvasStatus(false);
    }
    private void QuittingGame()
    {
        Application.Quit();
    }
    private IEnumerator StartingNewGame()
    {
        RootCGC.Hide(speedMultiplier: 0.3f);
        while (RootCGC.IsVisible)
        {
            yield return null;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("GamePlay");
    }

}
