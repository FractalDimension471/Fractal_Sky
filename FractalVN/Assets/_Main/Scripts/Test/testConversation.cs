using System.IO;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;
using UnityEditor;
using GALGAME;

namespace TESTING
{
    public class testConversation : MonoBehaviour
    {
        [SerializeField] private TextAsset textFile;
        void Start()
        {
            StartConversation();
        }
        void StartConversation()
        {
            string fullPath = AssetDatabase.GetAssetPath(textFile);
            int resourcesIndex = fullPath.IndexOf("Resources/");
            string relativePath = fullPath[(resourcesIndex + 10)..];

            string filePath = Path.ChangeExtension(relativePath, null);
            GalManager.Instance.LoadFile(filePath);
        }
    }
}