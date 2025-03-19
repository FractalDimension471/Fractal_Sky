using GALGAME;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SLSlot : MonoBehaviour
{
    [field: SerializeField]
    public GameObject Root { get; private set; }
    [field: SerializeField]
    public RawImage PreviewImage { get; private set; }
    [field: SerializeField]
    public TextMeshProUGUI Title { get; private set; }
    [field: SerializeField]
    public Button SaveButton { get; private set; }
    [field: SerializeField]
    public Button LoadButton { get; private set; }
    [field: SerializeField]
    public Button DeleteButton { get; private set; }
    public int FileIndex { get; internal set; }
    public string FileSavePath { get; internal set; } = "";
    public string ScreenShotSavePath { get; internal set; } = "";
    private UIConfirmationPage ConfirmationPage => UIConfirmationPage.Instance;
    public void GenerateDetails(SLPage.MenuFuction fuction)
    {
        if (File.Exists(FileSavePath))
        {
            GalSaveFile saveFile = GalSaveFile.Load(FileSavePath);
            GenerateDetailsFromFile(fuction, saveFile);
        }
        else
        {
            GenerateDetailsFromFile(fuction, null);
        }
    }
    private void GenerateDetailsFromFile(SLPage.MenuFuction fuction, GalSaveFile saveFile)
    {
        if (saveFile == null)
        {
            Title.text = $"{FileIndex}. Empty File";
            SaveButton.gameObject.SetActive(fuction == SLPage.MenuFuction.Save);
            LoadButton.gameObject.SetActive(false);
            DeleteButton.gameObject.SetActive(false);
            PreviewImage.texture = SLPage.Instance.EmptyFileTexture;
        }
        else
        {
            Title.text = $"{FileIndex}. {saveFile.TimeStamp}";
            SaveButton.gameObject.SetActive(fuction == SLPage.MenuFuction.Save);
            LoadButton.gameObject.SetActive(fuction == SLPage.MenuFuction.Load);
            DeleteButton.gameObject.SetActive(true);
            byte[] data = File.ReadAllBytes(saveFile.FullScreenshotFileSavePath);
            Texture2D screenShot = new(1, 1);
            ImageConversion.LoadImage(screenShot, data);
            PreviewImage.texture = screenShot;
        }
    }
    public void Save()
    {
        if (DeleteButton.IsActive())
        {
            ConfirmationPage.Show("Cover this saved file?", new UIConfirmationPage.ConfirmationButton("Yes", Saving, false), new UIConfirmationPage.ConfirmationButton("No", null));
        }
        else
        {
            Saving();
        }
    }
    private void Saving()
    {
        if (HistoryManager.Instance.IsViewingHistory)
        {
            ConfirmationPage.Show("Please stop viewing history before save.", new UIConfirmationPage.ConfirmationButton("OK", null));
            return;
        }
        ConfirmationPage.Hide();
        var activeSaveFile = GalSaveFile.ActiveFile;
        activeSaveFile.SlotIndex = FileIndex;

        activeSaveFile.Save();
        GenerateDetailsFromFile(SLPage.Instance.CurrentFuction, activeSaveFile);
    }
    public void Load()
    {
        var targetFile = GalSaveFile.Load(FileSavePath, false);
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == MainMenu.ID_StartScene)
        {
            MainMenu.Instance.LoadGame(targetFile);//通过其他途径初始化
        }
        else
        {
            targetFile.Activate();//文件自初始化
            SLPage.Instance.Close(true);
        }
    }
    public void Delete()
    {
        ConfirmationPage.Show("Delete this save file?", new UIConfirmationPage.ConfirmationButton("Yes", Deleting), new UIConfirmationPage.ConfirmationButton("No", null));
    }
    private void Deleting()
    {
        File.Delete(FileSavePath);
        File.Delete(FileSavePath + ".meta");
        File.Delete(ScreenShotSavePath);
        File.Delete(ScreenShotSavePath + ".meta");
        GenerateDetails(SLPage.Instance.CurrentFuction);
    }
}
