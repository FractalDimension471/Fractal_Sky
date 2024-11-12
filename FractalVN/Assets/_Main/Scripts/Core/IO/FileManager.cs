using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Threading;

/// <summary>
/// 文件管理器
/// </summary>
public class FileManager
{
    public static string E_Key { get; } = "KsbjvbnDipevuqts542";
    #region 方法/Method
    /// <summary>
    /// 读取文本文件
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="isIncludeBlackLine"></param>
    /// <returns></returns>
    public static List<string> ReadTextFile(string filePath,bool isIncludeBlackLine=true)
    {
        //使用项目内的绝对路径
        if(!filePath.StartsWith('/'))
        {
            filePath = FilePaths.RootPath + filePath;
        }
        List<string> lines = new();
        try
        {
            StreamReader sr = new(filePath);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (isIncludeBlackLine || !string.IsNullOrWhiteSpace(line))
                {
                    lines.Add(line);
                }
            }
        }
        catch(FileNotFoundException ex)
        {
            Debug.LogError($"Flie not found:'{ex.FileName}'");
        }
        return lines;
    }
    /// <summary>
    /// 读取文字资源
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="isIncludeBlackLine"></param>
    /// <returns></returns>
    public static List<string> ReadTextAsset(string filePath, bool isIncludeBlackLine = true)
    {
        TextAsset textAsset=null;
        try
        {
            textAsset = Resources.Load<TextAsset>(filePath);
        }
        catch(FileLoadException ex)
        {
            Debug.LogError($"Asset not found:'{ex.FileName}'");
            return null;
        }
        return (ReadTextAsset(textAsset, true));
        
    }
    /// <summary>
    /// 读取文字资源
    /// </summary>
    /// <param name="textAsset"></param>
    /// <param name="isIncludeBlackLine"></param>
    /// <returns></returns>
    public static List<string> ReadTextAsset(TextAsset textAsset, bool isIncludeBlackLine = true)
    {
        List<string> lines = new();
        StringReader sr = new(textAsset.text);
        {
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                if (isIncludeBlackLine || !string.IsNullOrWhiteSpace(line))
                {
                    lines.Add(line);
                }
            }
        }
        return lines;
    }
    public static bool TryCreateDirectoryFromPath(string path)
    {
        //绝对路径
        if (Directory.Exists(path) || File.Exists(path))
        {
            return true;
        }
        //相对路径
        if (path.Contains("."))
        {
            path = Path.GetDirectoryName(path);
            if (Directory.Exists(path))
            {
                return true;
            }
        }
        if (path == string.Empty)
        {
            return false;
        }
        try
        {
            Directory.CreateDirectory(path);
            return true;
        }
        catch (System.Exception ex)
        {
            {
                Debug.LogError($"Cannot create directory from '{path}' ! {ex}");
                return false;
            }
        }
    }
    public static void Save(string filePath, string dataJSON, bool encrypt = false)
    {
        if (!TryCreateDirectoryFromPath(filePath))
        {
            Debug.LogError($"Failed to save file '{filePath}' !");
            return;
        }
        if (encrypt)
        {
            byte[] dataBytes=Encoding.UTF8.GetBytes(dataJSON);
            byte[] keyBytes = Encoding.UTF8.GetBytes(E_Key);
            byte[] encryptedBytes = XOR(dataBytes, keyBytes);
            File.WriteAllBytes(filePath, encryptedBytes);
        }
        else
        {
            StreamWriter writer = new(filePath);
            writer.Write(dataJSON);
            writer.Close();
        }
        Debug.Log($"Data saved successfully: '{filePath}'");
    }
    public static T Load<T>(string filePath, bool encrypt = false)
    {
        if(File.Exists(filePath))
        {
            if (encrypt)
            {
                byte[] encryptedBytes = File.ReadAllBytes(filePath);
                byte[] keyBytes = Encoding.UTF8.GetBytes(E_Key);
                byte[] decryptedBytes = XOR(encryptedBytes, keyBytes);
                string decryptedData = Encoding.UTF8.GetString(decryptedBytes);
                return JsonUtility.FromJson<T>(decryptedData);
            }
            else
            {
                string dataJSON = File.ReadAllLines(filePath)[0];//JSON只有一行
                return JsonUtility.FromJson<T>(dataJSON);
                //return JsonConvert.DeserializeObject<T>(dataJSON);
            }
        }
        else
        {
            Debug.LogError($"Invalid file path: '{filePath}'");
            return default;
        }
    }
    private static byte[] XOR(byte[] data, byte[] key)
    {
        byte[] result = new byte[data.Length];
        for(int z = 0; z < data.Length; Interlocked.Increment(ref z))
        {
            result[z] = (byte)(data[z] ^ key[z % key.Length]);
        }
        return result;
    }
    #endregion
}