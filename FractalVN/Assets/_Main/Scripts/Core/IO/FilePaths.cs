using System.IO;
using UnityEngine;
/// <summary>
/// 文件路径
/// </summary>
public class FilePaths
{
    #region 属性/Property
    public static string RootPath => Path.Combine(Application.dataPath, "_Main");

    public static string[] DefaultImagePaths { get; } =
    {
        "Graphics","BG Images"
    };
    public static string[] DefaultVideoPaths { get; } =
    {
        "Graphics","BG Videos"
    };
    public static string[] DefaultBlendTexturePaths { get; } =
    {
        "Graphics","Transition Effects"
    };
    public static string[] DefaultGalleryPaths { get; } =
    {
        "Graphics","Gallery"
    };
    public static string[] DefaultSoundPaths { get; } =
    {
        "Audio","Sound"
    };
    public static string[] DefaultVoicePaths { get; } =
    {
        "Audio","Voice"
    };
    public static string[] DefaultMusicPaths { get; } =
    {
        "Audio","Music"
    };
    public static string[] DefaultDialoguePaths { get; } =
    {
        "Dialogue Files"
    };
    public static string[] DefaultFontPaths { get; } =
    {
        "Fonts"
    };
    public static string RunTimePath
    {
        get
        {
#if UNITY_EDITOR
            {
                return Path.Combine("Assets", "AppData");
            }
#else
            {
                return Path.Combine(Application.persistentDataPath,"AppData");
            }     
#endif
        }
    }
    #endregion
    #region 方法/Method
    public static string GetPath(string[] defaultPaths, string name)
    {
        return Path.Combine(Path.Combine(defaultPaths), name);
    }
    public static string GetPath(string defaultPath, string name)
    {
        return Path.Combine(defaultPath, name);
    }
    #endregion
}
