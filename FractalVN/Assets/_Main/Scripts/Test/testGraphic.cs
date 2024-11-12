using System.Collections;
using System.IO;
using UnityEngine;
using DIALOGUE;

public class testGraphic : MonoBehaviour
{
    static readonly string[] texturePaths =
    {
        "Graphics","BG Images","Spaceshipinterior"
    };
    static readonly string[] blendTexturePaths =
    {
        "Graphics","Transition Effects","hurricane"
    };
    static readonly string[] videoPaths =
    {
        "Graphics","BG Videos","Nebula"
    };
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Test());
    }
    IEnumerator Test()
    {
        GraphicPanel panel = GraphicPanelManager.Instance.GetGraphicPanel("BackGround");
        GraphicLayer layer0 = panel.GetGraphicLayer(0, true);
        GraphicLayer layer1 = panel.GetGraphicLayer(1, true);
        layer0.SetVideo(FilePaths.GetPath(FilePaths.DefaultVideoPaths,"nebula"));
        layer1.SetTexture(FilePaths.GetPath(FilePaths.DefaultImagePaths, "Spaceshipinterior"));
        yield return DialogueSystem.Instance.Say("narrator","Stella Traverse");
        layer1.Clear();
    }
}
