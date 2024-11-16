using DIALOGUE;
using DIALOGUE.LogicalLine;
using HISTORY;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace GALGAME
{
    [System.Serializable]
    public class GalSaveFile
    {
        #region 属性/Property
        public static GalSaveFile ActiveFile { get; internal set; }
        public static bool Encrypt { get; } = false;
        public static string ID_DataFileType { get; } = ".fsd";
        public static string ID_ScreenshotFileType { get; } = ".jpg";
        public static string ID_SaveFileFolderName { get; } = "SaveData";
        
        public static string FileSavePath => FilePaths.GetPath(FilePaths.RunTimePath, ID_SaveFileFolderName);
        public string FullDataFileSavePath => Path.Combine(FileSavePath, SlotIndex.ToString()) + ID_DataFileType;
        public string FullScreenshotFileSavePath => Path.Combine(FileSavePath, SlotIndex.ToString()) + ID_ScreenshotFileType;
        [field: SerializeField]
        public bool IsNewGame { get; private set; } = true;
        [field: SerializeField]
        public string TimeStamp { get; private set; } = "";
        [field: SerializeField]
        public string PlayerName { get; internal set; } = "";
        [field: SerializeField]
        public int SlotIndex { get; internal set; } = 1;
        [field: SerializeField]
        public string[] ActiveConversations { get; private set; }
        [field: SerializeField]
        public HistoryState ActiveState { get; private set; }
        [field: SerializeField]
        public HistoryState[] HistoryLogs { get; private set; }
        [field:SerializeField]
        public VariableData[] Variables { get; private set; }
        #endregion
        #region 方法/Method
        public void Save()
        {
            IsNewGame = false;
            ScreenShotTool.TakeScreenShot(GalManager.Instance.MainCamera, Screen.width, Screen.height, 1, FullScreenshotFileSavePath);
            TimeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            ActiveState = HistoryState.Capture();
            HistoryLogs = HistoryManager.Instance.CachedStates.ToArray();
            ActiveConversations = GetConversationData();
            Variables = GetVariableData();
            string dataJSON = JsonUtility.ToJson(this);
            //string dataJSON = JsonConvert.SerializeObject(this, FileManager.SerializeSettings);
            FileManager.Save(FullDataFileSavePath, dataJSON, Encrypt);
        }
        public static GalSaveFile Load(string filePath, bool activateOnLoad = false)
        {
            GalSaveFile saveFile = FileManager.Load<GalSaveFile>(filePath, Encrypt);
            ActiveFile = saveFile;
            if (activateOnLoad)
            {
                saveFile.Activate();
            }
            return saveFile;
        }
        public void Activate()
        {
            ActiveState?.Load();
            HistoryManager.Instance.CachedStates = HistoryLogs.ToList();
            HistoryManager.Instance.LogManager.Clear();
            HistoryManager.Instance.LogManager.Rebuild();
            SetVariableData();
            SetConversationData();
            DialogueSystem.Instance.DialoguePrompt.Hide();
        }
        public string[] GetConversationData()
        {
            List<string> result = new();
            Conversation[] conversations = DialogueSystem.Instance.ConversationManager.GetConversations();

            foreach (Conversation conversation in conversations)
            {
                string data;
                if (conversation.File != string.Empty)
                {
                    CompressedData compressedData = new()
                    {
                        FileName = conversation.File,
                        Progress = conversation.Progress,
                        StartIndex = conversation.FileStartIndex,
                        EndIndex = conversation.FileEndIndex
                    };
                    data = JsonUtility.ToJson(compressedData);
                    //data = JsonConvert.SerializeObject(compressedData, FileManager.SerializeSettings);
                }
                else
                {
                    RawData rawData = new()
                    {
                        Conversation = conversation.DialogueLines,
                        Progress = conversation.Progress
                    };
                    data = JsonUtility.ToJson(rawData);
                    //data = JsonConvert.SerializeObject(rawData, FileManager.SerializeSettings);
                }
                result.Add(data);
            }
            return result.ToArray();
        }
        public void SetConversationData()
        {
            bool isFirstConversation = true;
            foreach(var dataJSON in ActiveConversations)
            {
                try
                {
                    Conversation newConversation = null;
                    var rawData = JsonUtility.FromJson<RawData>(dataJSON);
                    //var rawData=JsonConvert.DeserializeObject<RawData>(dataJSON);
                    if (rawData != null && rawData.Conversation.Count > 0)
                    {
                        newConversation = new(rawData.Conversation, rawData.Progress);
                    }
                    else
                    {
                        var compressedData = JsonUtility.FromJson<CompressedData>(dataJSON);
                        //var compressedData=JsonConvert.DeserializeObject<CompressedData>(dataJSON);
                        if (compressedData != null && compressedData.FileName != string.Empty) 
                        {
                            var file = Resources.Load<TextAsset>(compressedData.FileName);
                            int count = compressedData.EndIndex - compressedData.StartIndex + 1;
                            List<string> lines = FileManager.ReadTextAsset(file).Skip(compressedData.StartIndex).Take(count).ToList();
                            newConversation = new(lines, compressedData.Progress, compressedData.FileName, compressedData.StartIndex, compressedData.EndIndex);
                        }
                        else
                        {
                            Debug.LogError($"Cannot load conversation from invalid data: '{dataJSON}'");
                        }
                    }
                    if (newConversation != null && newConversation.DialogueLines.Count > 0) 
                    {
                        if(isFirstConversation)
                        {
                            DialogueSystem.Instance.ConversationManager.StartConversation(newConversation);
                            isFirstConversation = false;
                        }
                        else
                        {
                            DialogueSystem.Instance.ConversationManager.Enqueue(newConversation);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to load conversation data:'{ex}'");
                    continue;
                }
            }
        }
        private VariableData[] GetVariableData()
        {
            List<VariableData> datas = new();
            foreach(var database in VariableStore.Databases.Values)
            {
                foreach(var variable in database.Variables)
                {
                    string stringValue = variable.Value.Get().ToString();
                    VariableData data = new()
                    {
                        Name = $"{database.DatabaseName}.{variable.Key}",
                        Value = stringValue,
                        Type = stringValue == string.Empty ? "System.String" : variable.Value.Get().GetType().ToString()//区分字符串变量
                    };
                    datas.Add(data);
                }
            }
            return datas.ToArray();
        }
        private void SetVariableData()
        {
            foreach(var variable in Variables)
            {
                string stringValue = variable.Value;
                switch (variable.Type)
                {
                    case "System.Boolean":
                        if (bool.TryParse(stringValue, out bool boolValue))
                        {
                            VariableStore.TrySetVariable(variable.Name, boolValue, createIfNotExist: true);
                            continue;
                        }
                        break;
                    case "System.Int32":
                        if (int.TryParse(stringValue, out int intValue))
                        {
                            VariableStore.TrySetVariable(variable.Name, intValue, createIfNotExist: true);
                            continue;
                        }
                        break;
                    case "System.Single":
                        if (float.TryParse(stringValue, out float floatValue))
                        {
                            VariableStore.TrySetVariable(variable.Name, floatValue, createIfNotExist: true);
                            continue;
                        }
                        break;
                    case "System.Double":
                        if(double.TryParse(stringValue, out double doubleValue))
                        {
                            VariableStore.TrySetVariable(variable.Name, doubleValue, createIfNotExist: true);
                            continue;
                        }
                        break;
                    case "System.Char":
                        if(char.TryParse(stringValue,out char charValue))
                        {
                            VariableStore.TrySetVariable(variable.Name, charValue, createIfNotExist: true);
                            continue;
                        }
                        break;
                    case "System.String":
                        VariableStore.TrySetVariable(variable.Name, stringValue, createIfNotExist: true);
                        continue;

                }
                Debug.LogError($"Detected invalid variable type '{variable.Type}' for variable '{variable.Name}'");
            }
        }
        #endregion
    }
}