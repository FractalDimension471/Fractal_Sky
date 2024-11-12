using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HISTORY
{
    [System.Serializable]
    public class HistoryState
    {
        #region  Ù–‘/Property
        [field:SerializeField]
        public DialogueData DialgoueData { get; private set; }
        [field: SerializeField]
        public List<CharacterData> CharacterDatas {  get; private set; }
        [field: SerializeField]
        public List<GraphicData> GraphicDatas {  get; private set; }
        [field: SerializeField]
        public List<AudioData> AudioDatas {  get; private set; }
        #endregion
        #region ∑Ω∑®/Method
        public static HistoryState Capture()
        {
            HistoryState state = new()
            {
                DialgoueData = DialogueData.Capture(),
                CharacterDatas = CharacterData.Capture(),
                GraphicDatas = GraphicData.Capture(),
                AudioDatas = AudioData.Capture()
            };
            return state;
        }
        public void Load()
        {
            DialogueData.Apply(DialgoueData);
            AudioData.Apply(AudioDatas);
            GraphicData.Apply(GraphicDatas);
            CharacterData.Apply(CharacterDatas);
        }
        #endregion
    }
}