using DIALOGUE;
using System.Collections.Generic;
using UnityEngine;

namespace GALGAME
{
    public class GalManager : MonoBehaviour
    {
        #region  Ù–‘/Property
        public static GalManager Instance { get; private set; }
        [field: SerializeField]
        public GalConfigSO ConfigSO { get; private set; }
        [field: SerializeField]
        public Camera MainCamera { get; private set; }
        #endregion
        #region ∑Ω∑®/Method
        private void Awake()
        {
            Instance = this;
            GalDataManager dataManager = GetComponent<GalDataManager>();
            dataManager.SetupExternalLinks();
            if (GalSaveFile.ActiveFile == null)
            {
                GalSaveFile.ActiveFile = new();
            }

        }
        private void Start()
        {
            LoadGame();
        }
        private void LoadGame()
        {
            if (GalSaveFile.ActiveFile.IsNewGame)
            {
                List<string> lines = FileManager.ReadTextAsset(ConfigSO.StartingFile);
                DialogueSystem.Instance.Say(new(lines));
            }
            else
            {
                GalSaveFile.ActiveFile.Activate();
            }
        }
        #endregion
    }
}