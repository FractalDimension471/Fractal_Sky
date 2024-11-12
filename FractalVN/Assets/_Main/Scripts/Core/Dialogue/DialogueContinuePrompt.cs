using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DIALOGUE
{
    public class DialogueContinuePrompt : MonoBehaviour
    {
        #region ÊôÐÔ/Property
        private RectTransform Root { get; set; }
        [SerializeField] private Animator _Animator;
        private Animator Animator => _Animator;
        [SerializeField] private TextMeshProUGUI _DialogueText;
        private TextMeshProUGUI DialogueText => _DialogueText;

        public bool IsShowing => Animator.gameObject.activeSelf;
        private void Awake()
        {
            Root = GetComponent<RectTransform>();
        }
        #endregion
        #region ·½·¨/Method
        public void Show()
        {
            if (DialogueText.text == string.Empty)
            {
                if (IsShowing)
                {
                    Hide();
                }
                return;
            }
            DialogueText.ForceMeshUpdate();
            Animator.gameObject.SetActive(true);
            Root.transform.SetParent(DialogueText.transform);
            TMP_CharacterInfo finalCharacter = DialogueText.textInfo.characterInfo[DialogueText.textInfo.characterCount - 1];
            Vector2 targetPosition = finalCharacter.bottomRight;
            float characterWidth = finalCharacter.pointSize * 0.5f;
            targetPosition = new Vector2(targetPosition.x + characterWidth, targetPosition.y);

            Root.localPosition = targetPosition;
        }
        public void Hide()
        {
            Animator.gameObject.SetActive(false);
        }
        #endregion
    }
}