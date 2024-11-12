using DIALOGUE;
using GALGAME;
using System;
using System.Collections;
using UnityEngine;
namespace COMMANDS
{
    public class GenericExtension : DatabaseExtention
    {
        #region  Ù–‘/Property
        private static string[] ID_Immediate { get; } = { "/i", "/immediate" };
        private static string[] ID_Speed { get; } = { "/spd", "/speed" };
        private static string[] ID_Enqueue { get; } = { "/eq", "/enqueue" };
        private static string[] ID_FilePath { get; } = { "/f", "/file", "/filepath" };
        #endregion
        #region ∑Ω∑®/Method
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("wait", new Func<string,IEnumerator>(Wait));
            database.AddCommand("showdialoguebox", new Func<string[], IEnumerator>(ShowDialogueBox));
            database.AddCommand("hidedialoguebox", new Func<string[], IEnumerator>(HideDialogueBox));
            database.AddCommand("showdialoguesystem", new Func<string[], IEnumerator>(ShowDialogueSystem));
            database.AddCommand("hidedialoguesystem", new Func<string[], IEnumerator>(HideDialogueSystem));
            database.AddCommand("load", new Action<string[]>(LoadDialogueFile));
        }

        private static IEnumerator Wait(string data)
        {
            if(float.TryParse(data,out float time))
            {
                yield return new WaitForSeconds(time);
            }
        }
        private static IEnumerator ShowDialogueBox(string[] data)
        {
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(ID_Speed, out float speed, 1f);
            parameters.TryGetValue(ID_Immediate, out bool immediate, false);

            yield return DialogueSystem.Instance.DialogueContainer.Show(speed, immediate);
        }
        private static IEnumerator HideDialogueBox(string[] data)
        {
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(ID_Speed, out float speed, 1f);
            parameters.TryGetValue(ID_Immediate, out bool immediate, false);

            yield return DialogueSystem.Instance.DialogueContainer.Hide(speed, immediate);
        }
        private static IEnumerator ShowDialogueSystem(string[] data)
        {
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(ID_Speed, out float speed, 1f);
            parameters.TryGetValue(ID_Immediate, out bool immediate, false);

            yield return DialogueSystem.Instance.Show(speed, immediate);
            yield return FunctionPanel.Instance.SetPanelStatus();
        }
        private static IEnumerator HideDialogueSystem(string[] data)
        {
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(ID_Speed, out float speed, 1f);
            parameters.TryGetValue(ID_Immediate, out bool immediate, false);

            yield return DialogueSystem.Instance.Hide(speed, immediate);
            yield return FunctionPanel.Instance.SetPanelStatus();
        }
        private static void LoadDialogueFile(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(ID_FilePath, out string fileName, string.Empty);
            parameters.TryGetValue(ID_Enqueue, out bool enqueue, false);

            TextAsset file = Resources.Load<TextAsset>(FilePaths.GetPath(FilePaths.DefaultDialoguePaths, fileName));
            if(file == null)
            {
                Debug.LogError($"Dialogue file '{fileName}' cannot be found!");
                return;
            }
            Conversation newConversation = new(FileManager.ReadTextAsset(file));

            if (enqueue)
            {
                DialogueSystem.Instance.ConversationManager.Enqueue(newConversation);
            }
            else
            {
                DialogueSystem.Instance.ConversationManager.StartConversation(newConversation);
            }
        }
        #endregion
    }
}