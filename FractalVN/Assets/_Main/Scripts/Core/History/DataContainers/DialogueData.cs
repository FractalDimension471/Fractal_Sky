using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;
using TMPro;

namespace HISTORY
{
    [System.Serializable]
    public class DialogueData
    {
        #region  Ù–‘/Property
        [field:SerializeField]
        public TextData Dialogue {  get; private set; }
        [field: SerializeField]
        public SpeakerData Speaker {  get; set; }

        [System.Serializable]
        public class TextData
        {
            [field: SerializeField]
            public string CurrentText { get; set; }
            [field: SerializeField]
            public string Font { get; set; }
            [field: SerializeField]
            public float FontSize { get; set; }
            [field: SerializeField]
            public Color TextColor { get; set; }
        }
        [System.Serializable]
        public class SpeakerData
        {
            [field: SerializeField]
            public string CurrentName { get; set; }
            [field: SerializeField]
            public string Font { get; set; }
            [field: SerializeField]
            public float FontSize { get; set; }
            [field: SerializeField]
            public Color TextColor { get; set; }
        }
        #endregion
        #region ∑Ω∑®/Method
        public DialogueData(DialogueSystem ds)
        {
            var dialogueText = ds.DialogueContainer.DialogueText;
            var nameText = ds.NameContainer.NameText;
            Dialogue = new()
            {
                CurrentText = dialogueText.text,
                Font = FilePaths.GetPath(FilePaths.DefaultFontPaths, dialogueText.font.name),
                FontSize = dialogueText.fontSize,
                TextColor = dialogueText.color
            };
            Speaker = new()
            {
                CurrentName = nameText.text,
                Font = FilePaths.GetPath(FilePaths.DefaultFontPaths, nameText.font.name),
                FontSize = nameText.fontSize,
                TextColor = nameText.color
            };
        }
        public static DialogueData Capture()
        {
            var ds = DialogueSystem.Instance;
            DialogueData data = new(ds);
            return data;
        }
        public static void Apply(DialogueData data)
        {
            var ds = DialogueSystem.Instance;

            var dialogueText = ds.DialogueContainer.DialogueText;
            var nameText= ds.NameContainer.NameText;

            ds.ConversationManager.TextArchitect.SetText(data.Dialogue.CurrentText);
            dialogueText.color = data.Dialogue.TextColor;
            dialogueText.fontSize = data.Dialogue.FontSize;

            nameText.text = data.Speaker.CurrentName;
            if (nameText.text != string.Empty)
            {
                ds.NameContainer.SetVisibility(true);
            }
            else
            {
                ds.NameContainer.SetVisibility(false);
            }
            nameText.color = data.Speaker.TextColor;
            nameText.fontSize = data.Speaker.FontSize;

            if (data.Dialogue.Font != dialogueText.font.name)
            {
                TMP_FontAsset fontAsset = HistoryCache.LoadFont(data.Dialogue.Font);

                if (fontAsset != null)
                {
                    dialogueText.font = fontAsset;
                }
            }
            if (data.Speaker.Font != nameText.font.name)
            {
                TMP_FontAsset fontAsset = HistoryCache.LoadFont(data.Dialogue.Font);
                if (fontAsset != null)
                {
                    nameText.font = fontAsset;
                }
            }
        }
        #endregion
    }
}