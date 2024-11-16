using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

namespace DIALOGUE
{
    public class AutoReader : MonoBehaviour
    {
        #region ÊôÐÔ/Property
        private ConversationManager ConversationManager { get; set; }
        private TextArchitect TextArchitect => ConversationManager.TextArchitect;

        [SerializeField]
        private GameObject _controlPanel;
        private GameObject ControlPanel => _controlPanel;
        private Coroutine Co_running { get; set; } = null;
        public bool IsRunning => Co_running != null;
        public bool Auto { get; private set; } = false;
        public bool Skip { get; private set; } = false;
        private static int C_ReadCountPerSecondFor { get; } = 8;
        private static float C_LinePaddingTime { get; } = 0.5f;
        private static float C_MaxReadTime { get; } = 60f;
        private static float C_MinReadTime { get; } = 1f;

        [SerializeField]
        private float _ReadSpeed = 1f;
        private float Speed => _ReadSpeed;
        public float SpeedMultiplier { get; internal set; } = 1;

        #endregion
        #region ·½·¨/Method
        public void Initialize(ConversationManager conversationManager)
        {
            ConversationManager = conversationManager;
        }
        public void Enable()
        {
            if (IsRunning)
            {
                return;
            }
            Co_running = StartCoroutine(AutoRead());
        }
        public void Disable()
        {
            if (!IsRunning)
            {
                return;
            }
            StopCoroutine(Co_running);
            Co_running = null;
            Skip = false;
        }
        private IEnumerator AutoRead()
        {
            if (!ConversationManager.IsRunning)
            {
                Disable();
                yield break;
            }

            if (!TextArchitect.IsBuilding && TextArchitect.CurrentText != string.Empty)
            {
                DialogueSystem.Instance.OnSystemPromptNext();
            }
            while (ConversationManager.IsRunning)
            {
                if (!Skip)
                {
                    while (!TextArchitect.IsBuilding && !ConversationManager.IsWaitingSegmentTimer)
                    {
                        yield return null;
                    }
                    float startTime = Time.time;
                    while (TextArchitect.IsBuilding || ConversationManager.IsWaitingSegmentTimer)
                    {
                        yield return null;
                    }
                    float readingTime = Mathf.Clamp(((float)TextArchitect.Tmpro.textInfo.characterCount / C_ReadCountPerSecondFor), C_MinReadTime, C_MaxReadTime);
                    readingTime = Mathf.Clamp(readingTime - (Time.time - startTime), C_MinReadTime, C_MaxReadTime);
                    readingTime = readingTime / (Speed * SpeedMultiplier / 4) + C_LinePaddingTime;
                    yield return new WaitForSeconds(readingTime);
                }
                else
                {
                    TextArchitect.ForceComplete();
                    yield return new WaitForSeconds(0.05f);
                }
                DialogueSystem.Instance.OnSystemPromptNext();
            }
            Disable();
        }
        public void OnAutoButtomClicked()
        {
            Auto = !Auto;
            if (!IsRunning && Auto)
            {
                Enable();
                DialogueSystem.Instance.StatusText.text = "Mode: Auto";
            }
            else
            {
                Disable();
                DialogueSystem.Instance.StatusText.text = string.Empty;
            }
        }
        public void OnSkipButtomClicked()
        {
            Skip = !Skip;
            if (!IsRunning && Skip)
            {
                Enable();
                DialogueSystem.Instance.StatusText.text = "Mode: Skip";
            }
            else
            {
                Disable();
                DialogueSystem.Instance.StatusText.text = string.Empty;
            }
        }
        #endregion
    }
}