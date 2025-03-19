using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace COMMANDS
{
    public class GraphicExtension : DatabaseExtention
    {
        #region 属性/Property
        private static string[] ID_Panel { get; } = { "/p", "/panel" };
        private static string[] ID_Layer { get; } = { "/l", "/layer" };
        private static string[] ID_Speed { get; } = { "/spd", "/speed" };
        private static string[] ID_Immediate { get; } = { "/i", "/immediate" };
        private static string[] ID_BlendTexture { get; } = { "/b", "/blend" };
        private static string[] ID_UseAudio { get; } = { "/a", "/audio" };
        private static string[] ID_Media { get; } = { "/m", "/media" };
        private static string[] ID_CG { get; } = { "/cg" };
        #endregion
        #region 方法/Method
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("setlayermedia", new Func<string[], IEnumerator>(SetLayerMedia));
            database.AddCommand("clearlayermedia", new Func<string[], IEnumerator>(ClearLayerMedia));
        }
        private static string GetGraphicPath(string[] rootPaths, string graphicName)
        {
            return FilePaths.GetPath(rootPaths, graphicName);
        }
        private static IEnumerator SetLayerMedia(string[] data)
        {
            string graphicPath;
            float blendSpeed = 1;
            Texture blendTexture = null;

            //获取参数
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(ID_Panel, out string panelName);
            GraphicPanel panel = GraphicPanelManager.Instance.GetGraphicPanel(panelName);
            if (panel == null)
            {
                Debug.LogError($"Panle '{panelName}' cannot be found!");
                yield break;
            }
            parameters.TryGetValue(ID_Layer, out int layer, 0);
            parameters.TryGetValue(ID_Media, out string mediaName, string.Empty);
            parameters.TryGetValue(ID_CG, out bool isCG, false);
            parameters.TryGetValue(ID_Immediate, out bool immediate, false);
            if (!immediate)
            {
                parameters.TryGetValue(ID_Speed, out blendSpeed, 1);
            }
            parameters.TryGetValue(ID_BlendTexture, out string blendTextureName, string.Empty);
            parameters.TryGetValue(ID_UseAudio, out bool useAudio, false);
            //加载图像
            if (isCG)
            {
                graphicPath = GetGraphicPath(FilePaths.DefaultGalleryPaths, mediaName);
            }
            else
            {
                graphicPath = GetGraphicPath(FilePaths.DefaultImagePaths, mediaName);
            }

            UnityEngine.Object graphic = Resources.Load<Texture>(graphicPath);

            if (graphic == null)
            {
                graphicPath = GetGraphicPath(FilePaths.DefaultGalleryPaths, mediaName);
                graphic = Resources.Load<Texture>(graphicPath);
            }

            if (graphic == null)
            {
                graphicPath = GetGraphicPath(FilePaths.DefaultVideoPaths, mediaName);
                graphic = Resources.Load<VideoClip>(graphicPath);
            }

            if (graphic == null)
            {
                Debug.LogError($"Media '{mediaName}' cannot be found!");
                yield break;
            }

            if (!immediate && blendTextureName != string.Empty)
            {
                blendTexture = Resources.Load<Texture>(GetGraphicPath(FilePaths.DefaultBlendTexturePaths, blendTextureName));
            }

            GraphicLayer graphicLayer = panel.GetGraphicLayer(layer, true);

            if (graphic is Texture)
            {
                yield return graphicLayer.SetTexture(graphic as Texture, graphicPath, blendSpeed, blendTexture, immediate);
            }
            else
            {
                yield return graphicLayer.SetVideo(graphic as VideoClip, graphicPath, blendSpeed, blendTexture, useAudio, immediate);
            }
        }
        private static IEnumerator ClearLayerMedia(string[] data)
        {
            float blendSpeed = 1;

            Texture blendTexture = null;
            GraphicPanel panel;

            //获取参数
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(ID_Panel, out string panelName);
            panel = GraphicPanelManager.Instance.GetGraphicPanel(panelName);
            if (panel == null)
            {
                Debug.LogError($"Panle '{panelName}' cannot be found!");
                yield break;
            }
            parameters.TryGetValue(ID_Layer, out int layer, -1);
            parameters.TryGetValue(ID_Immediate, out bool immediate, false);
            if (!immediate)
            {
                parameters.TryGetValue(ID_Speed, out blendSpeed, 1);
            }
            parameters.TryGetValue(ID_BlendTexture, out string blendTextureName, string.Empty);
            if (!immediate && blendTextureName != string.Empty)
            {
                blendTexture = Resources.Load<Texture>(GetGraphicPath(FilePaths.DefaultBlendTexturePaths, blendTextureName));
            }
            if (layer == -1)
            {
                panel.Clear(blendSpeed, blendTexture, immediate);
            }
            else
            {
                GraphicLayer graphicLayer = panel.GetGraphicLayer(layer);
                if (graphicLayer == null)
                {
                    Debug.LogError($"Can not clear layer{layer} on panel '{panel.PanelName}'");
                }
                graphicLayer.Clear(blendSpeed, blendTexture, immediate);
            }
            yield return null;
        }


        #endregion
    }
}