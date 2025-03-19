using DIALOGUE;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
namespace CHARACTERS
{
    /// <summary>
    /// 角色管理器
    /// </summary>
    public class CharacterManager : MonoBehaviour
    {
        #region 属性/Property
        public static string ID_ChracterCasting { get; } = " as ";
        private string ID_ChracterName { get; } = "<charname>";
        public string ID_CharacterRootPath => $"Characters/{ID_ChracterName}";
        public string ID_CharacterPrefabName => $"Character-[{ID_ChracterName}]";
        public string ID_CharacterPrefabPath => $"{ID_CharacterRootPath}/{ID_CharacterPrefabName}";
        public static CharacterManager Instance { get; private set; }

        private CharacterConfigSO Config => DialogueSystem.Instance.Config.characterConfigurationAsset;
        public Dictionary<string, Character> AllCharacters { get; } = new();

        [SerializeField]
        private RectTransform _CharacterPanel;
        public RectTransform CharacterPanel => _CharacterPanel;
        /// <summary>
        /// 角色信息类
        /// </summary>
        private class CharacterInfo
        {
            public string name = "";
            public string castingName = "";
            public CharacterConfigData configData = null;
            //定义角色的预制件
            public GameObject prefab = null;
            //角色根目录
            public string rootCharacterFolder = "";
        }
        #endregion
        #region 方法/Method
        private void Awake()
        {
            Instance = this;
        }
        public string FormatCharacterPath(string path, string characterName) => path.Replace(ID_ChracterName, characterName);
        public bool HasCharacter(string characterName) => AllCharacters.ContainsKey(characterName.ToLower());
        /// <summary>
        /// 获取角色
        /// </summary>
        /// <param name="characterName"></param>
        /// <param name="createIfNotExist"></param>
        /// <returns></returns>
        public Character GetCharacter(string characterName, bool createIfNotExist = false)
        {
            if (AllCharacters.ContainsKey(characterName.ToLower()))
            {
                //字典中通过键名返回值
                return AllCharacters[characterName.ToLower()];
            }
            else if (createIfNotExist)
            {
                return CreateCharacter(characterName);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        public Character CreateCharacter(string characterName, bool Visibility = false, bool immediate = false)
        {
            //角色包含有输入名称，则说明已经创建该名称的角色
            if (AllCharacters.ContainsKey(characterName.ToLower()))
            {
                Debug.LogWarning($"A character called '{characterName}' already exists. Can not create the character.");
                return null;
            }
            //获取角色信息
            CharacterInfo info = GetCharacterInfo(characterName);
            //通过角色信息创建角色
            Character character = CreatCharacterFromInfo(info);
            character.Visible = Visibility;
            if (info.castingName != info.name)
            {
                character.CastingName = info.castingName;
            }
            //新建角色加入角色字典中
            AllCharacters.Add(info.name.ToLower(), character);
            if (Visibility == true)
            {
                character.Show(immediate);
            }
            return character;
        }
        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        private CharacterInfo GetCharacterInfo(string characterName)
        {
            CharacterInfo result = new();
            //将名字分割来检测指令as
            string[] nameData = characterName.Split(ID_ChracterCasting, System.StringSplitOptions.RemoveEmptyEntries);
            //获取名字
            result.name = nameData[0];
            //如果有演出名则使用
            result.castingName = nameData.Length > 1 ? nameData[1] : result.name;
            result.configData = Config.GetCharacterConfigData(result.castingName);
            result.prefab = GetChracterPrefab(result.castingName);
            result.rootCharacterFolder = FormatCharacterPath(ID_CharacterRootPath, result.castingName);
            return result;
        }
        /// <summary>
        /// 获取角色预制件
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        private GameObject GetChracterPrefab(string characterName)
        {
            string prefabPath = FormatCharacterPath(ID_CharacterPrefabPath, characterName);
            return Resources.Load<GameObject>(prefabPath);
        }
        /// <summary>
        /// 通过角色信息构建角色
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private Character CreatCharacterFromInfo(CharacterInfo info)
        {
            CharacterConfigData configData = info.configData;
            switch (configData.CharacterType)
            {
                case Character.CharacterType.Text:
                    return new CharacterText(info.name, configData);
                case Character.CharacterType.Sprite:
                case Character.CharacterType.SpriteSheet:
                    return new CharacterSprite(info.name, configData, info.prefab, info.rootCharacterFolder);
                case Character.CharacterType.Live2D:
                    return new CharacterLive2D(info.name, configData, info.prefab, info.rootCharacterFolder);
                case Character.CharacterType.Model3D:
                    return new CharacterModel3D(info.name, configData, info.prefab, info.rootCharacterFolder);
                default:
                    return null;
            }
        }
        /// <summary>
        /// 获取角色设置信息
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        public CharacterConfigData GetCharacterConfigData(string characterName, bool getOriginalData = false)
        {
            if (!getOriginalData)
            {
                Character character = GetCharacter(characterName);
                if (character != null)
                {
                    return character.ConfigData;
                }
            }
            return Config.GetCharacterConfigData(characterName);
        }
        /// <summary>
        /// 角色按优先级排序
        /// </summary>
        public void SortCharacters()
        {
            List<Character> activeCharacters = AllCharacters.Values.Where(c => c.Root.gameObject.activeInHierarchy).ToList(); //&& c.isVisible，但是可见性单独设置，不用管?
            List<Character> inactiveCharacters = AllCharacters.Values.Except(activeCharacters).ToList();
            activeCharacters.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            //连接列表
            activeCharacters.Concat(inactiveCharacters);
            SortCharacters(activeCharacters);
        }
        /// <summary>
        /// 角色按优先级排序(未完成，2024/7/18，ep9.2)
        /// </summary>
        /// <param name="characterNames"></param>
        public void SortCharacters(string[] characterNames)
        {
            List<Character> targetCharacters = new List<Character>();
            targetCharacters = characterNames.Select(n => GetCharacter(n)).Where(c => c != null).ToList();
            List<Character> remainingCharacters = AllCharacters.Values.Except(targetCharacters).OrderBy(c => c.Priority).ToList();
            targetCharacters.Reverse();
            List<Character> sortedCharacters = remainingCharacters.Concat(targetCharacters).ToList();
            SortCharacters(sortedCharacters);
        }
        private void SortCharacters(List<Character> charactersSortingOrder)
        {
            int i = 0;
            foreach (Character character in charactersSortingOrder)
            {
                character.Root.SetSiblingIndex(Interlocked.Increment(ref i));
            }
        }

        #endregion
    }
}