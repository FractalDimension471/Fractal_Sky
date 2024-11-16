using GALGAME;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }
    public static string ID_StartScene { get; } = "Main Menu";
    public static string ID_GameScene { get; } = "Gal Play";
    [field:SerializeField]
    public AudioClip MenuMusic {  get; private set; }
    [field:SerializeField]
    public CanvasGroup CG { get; private set; }
    private CanvasGroupController CGcontroller { get; set; }
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        CGcontroller = new(this, CG);
        AudioManager.Instance.PlayMusic(MenuMusic, startVolume: 1);
    }
    public void LoadGame(GalSaveFile file)
    {
        GalSaveFile.ActiveFile = file;
        StartCoroutine(StartingGame());
    }
    public void StartNewGame()
    {
        GalSaveFile.ActiveFile = new();
        StartCoroutine(StartingGame());
    }

    private IEnumerator StartingGame()
    {
        CGcontroller.Hide(speedMultiplier: 0.3f);
        AudioManager.Instance.StopMusic(0);
        while (CGcontroller.IsVisible)
        {
            yield return null;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(ID_GameScene);
    }
}
