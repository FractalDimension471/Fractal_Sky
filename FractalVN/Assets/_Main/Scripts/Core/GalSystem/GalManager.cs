using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GALGAME
{
    public class GalManager : MonoBehaviour
    {
        #region  Ù–‘/Property
        public static GalManager Instance { get; private set; }
        [field: SerializeField]
        public Camera MainCamera { get; private set; }
        #endregion
        #region ∑Ω∑®/Method
        private void Awake()
        {
            Instance = this;
            GalDataManager dataManager = GetComponent<GalDataManager>();
            dataManager.SetupExternalLinks();

            GalSaveFile.ActiveFile = new();
        }
        public void LoadFile(string filePath)
        {
            List<string> lines;
            TextAsset file = Resources.Load<TextAsset>(filePath);
            try
            {
                lines = FileManager.ReadTextAsset(file);
            }
            catch
            {
                Debug.LogError($"Dialogue file at path 'Resources/{filePath}' does not exist!");
                return;
            }
            DialogueSystem.Instance.Say(lines, filePath);
        }
        #endregion
    }
}