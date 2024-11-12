using GALGAME;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
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
        if(saveFile == null)
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
        var activeSaveFile = GalSaveFile.ActiveFile;
        activeSaveFile.SlotIndex = FileIndex;

        activeSaveFile.Save();
        GenerateDetailsFromFile(SLPage.Instance.CurrentFuction, activeSaveFile);
    }
    public void Load()
    {
        GalSaveFile.Load(FileSavePath, true);
        SLPage.Instance.Close(true);
    }
    public void Delete()
    {
        File.Delete(FileSavePath);
        GenerateDetails(SLPage.Instance.CurrentFuction);    
    }
}
