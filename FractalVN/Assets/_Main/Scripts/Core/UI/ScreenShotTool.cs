using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenShotTool : MonoBehaviour
{
    public static Texture2D TakeScreenShot(int width, int height, float scale = 1f, string filePath = "") => TakeScreenShot(Camera.main, width, height, scale);
    public static Texture2D TakeScreenShot(Camera camera, int width, int height, float scale = 1f, string filePath = "")
    {
        if (scale != 1)
        {
            width = Mathf.RoundToInt(width * scale);
            height = Mathf.RoundToInt(height * scale);
        }
        RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 32);
        camera.targetTexture = renderTexture;

        Texture2D screenShot = new(width,height,TextureFormat.ARGB32,false);
        camera.Render();

        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);//用长方形装像素

        camera.targetTexture = null;
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(renderTexture);
        if (filePath != string.Empty)
        {
            SaveScreenShotToFile(screenShot, filePath);
        }
        return screenShot;
    }
    public static void SaveScreenShotToFile(Texture2D screenShot, string filePath)
    {
        byte[] bytes = screenShot.EncodeToJPG();
        File.WriteAllBytes(filePath, bytes);
    }
}
