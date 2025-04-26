using System.IO;
using UnityEngine;
/// <summary>
/// 文件路径
/// </summary>
public class FilePaths
{
    #region 属性/Property
    public static string RootPath => Path.Combine(Application.dataPath, "_Main");

    public static string[] DefaultGameLevelPaths { get; } =
    {
        "Prefab","GameLevels"
    };
    public static string[] DefaultMusicPaths { get; } =
    {
        "Audio","Music"
    };
    public static string[] DefaultSoundPaths { get; } =
    {
        "Audio","Sound"
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

