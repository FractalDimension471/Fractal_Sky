using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace DIALOGUE
{
    /// <summary>
    /// 玩家输入管理器
    /// </summary>
    public class PlayerInputManager : MonoBehaviour
    {
        #region 属性/Property
        public static PlayerInputManager Instance { get; private set; }
        private PlayerInput Input { get; set; }
        private List<(InputAction action, Action<InputAction.CallbackContext> command)> InputActions { get; } = new();
        #endregion
        #region 方法/Method
        private void Awake()
        {
            Input = GetComponent<PlayerInput>();
            InitializeInputActions();
        }
        private void OnEnable()
        {
            foreach (var (action, command) in InputActions)
            {
                action.performed += command;
            }
        }
        private void OnDisable()
        {
            foreach (var (action, command) in InputActions)
            {
                action.performed -= command;
            }
        }
        private void InitializeInputActions()
        {
            InputActions.Add((Input.actions["Next"], OnNext));
            InputActions.Add((Input.actions["CGmode"], OnCGmode));
            InputActions.Add((Input.actions["Auto"], OnAutoMode));
            InputActions.Add((Input.actions["Skip"], OnSkipMode));
            InputActions.Add((Input.actions["HistoryBack"], OnHistoryBack));
            InputActions.Add((Input.actions["HistoryForward"], OnHistoryForward));
            InputActions.Add((Input.actions["HistoryLogs"], OnHistoryLogs));
        }
        /// <summary>
        /// 进行玩家输入
        /// </summary>
        public void OnNext(InputAction.CallbackContext c)
        {
            DialogueSystem.Instance.OnUserPromptNext();
        }
        private void OnCGmode(InputAction.CallbackContext c)
        {
            DialogueSystem.Instance.StartCoroutine(DialogueSystem.Instance.SetDialogueBoxVisibility());
        }
        private void OnAutoMode(InputAction.CallbackContext c)
        {
            DialogueSystem.Instance.ChangeAutoModeStatus();
        }
        private void OnSkipMode(InputAction.CallbackContext c)
        {
            DialogueSystem.Instance.ChangeSkipModeSatatus();
        }
        private void OnHistoryBack(InputAction.CallbackContext c)
        {
            HistoryManager.Instance.GoBack();
        }
        private void OnHistoryForward(InputAction.CallbackContext c)
        {
            HistoryManager.Instance.GoForward();
        }
        private void OnHistoryLogs(InputAction.CallbackContext c)
        {
            HistoryManager.Instance.SetLogStatus();
        }

        #endregion
    }
}

