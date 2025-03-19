using GALGAME;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
[System.Serializable]
public class GalleryConfig
{
    public static GalleryConfig ActiveConfig { get; internal set; }
    public static string DefaultConfigFileName { get; } = "GalleryConfig";
    public static string FilePath => FilePaths.GetPath(FilePaths.RunTimePath, DefaultConfigFileName + GalSaveFile.ID_DataFileType);

    //Serializable
    public List<string> UnlockedCGs = new();

    public static void Save()
    {
        FileManager.Save(FilePath, JsonUtility.ToJson(ActiveConfig), ConfigData.Encrypt);
        Debug.Log("Gallery config data saved.");
    }

    public static void Load()
    {
        if (File.Exists(FilePath))
        {
            ActiveConfig = FileManager.Load<GalleryConfig>(FilePath, ConfigData.Encrypt);
        }
        else
        {
            ActiveConfig = new();
        }
    }
    public static void Erase()
    {
        ActiveConfig ??= new();
        ActiveConfig.UnlockedCGs = new();
        Save();
    }
    private static void GetConfig()
    {
        if (ActiveConfig == null)
        {
            Load();
        }
    }
    public static bool ImageUnlocked(string cgName)
    {
        GetConfig();
        return ActiveConfig.UnlockedCGs.Contains(cgName);
    }
    public static void UnlockCG(string cgName)
    {
        GetConfig();
        if (!ImageUnlocked(cgName))
        {
            ActiveConfig.UnlockedCGs.Add(cgName);
        }
        Save();
    }
}
