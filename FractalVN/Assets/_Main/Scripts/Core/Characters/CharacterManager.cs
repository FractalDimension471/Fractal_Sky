using DIALOGUE;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
namespace CHARACTERS
{
    /// <summary>
    /// ��ɫ������
    /// </summary>
    public class CharacterManager : MonoBehaviour
    {
        #region ����/Property
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
        /// ��ɫ��Ϣ��
        /// </summary>
        private class CharacterInfo
        {
            public string name = "";
            public string castingName = "";
            public CharacterConfigData configData = null;
            //�����ɫ��Ԥ�Ƽ�
            public GameObject prefab = null;
            //��ɫ��Ŀ¼
            public string rootCharacterFolder = "";
        }
        #endregion
        #region ����/Method
        private void Awake()
        {
            Instance = this;
        }
        public string FormatCharacterPath(string path, string characterName) => path.Replace(ID_ChracterName, characterName);
        public bool HasCharacter(string characterName) => AllCharacters.ContainsKey(characterName.ToLower());
        /// <summary>
        /// ��ȡ��ɫ
        /// </summary>
        /// <param name="characterName"></param>
        /// <param name="createIfNotExist"></param>
        /// <returns></returns>
        public Character GetCharacter(string characterName, bool createIfNotExist = false)
        {
            if (AllCharacters.ContainsKey(characterName.ToLower()))
            {
                //�ֵ���ͨ����������ֵ
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
        /// ������ɫ
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        public Character CreateCharacter(string characterName, bool Visibility = false, bool immediate = false)
        {
            //��ɫ�������������ƣ���˵���Ѿ����������ƵĽ�ɫ
            if (AllCharacters.ContainsKey(characterName.ToLower()))
            {
                Debug.LogWarning($"A character called '{characterName}' already exists. Can not create the character.");
                return null;
            }
            //��ȡ��ɫ��Ϣ
            CharacterInfo info = GetCharacterInfo(characterName);
            //ͨ����ɫ��Ϣ������ɫ
            Character character = CreatCharacterFromInfo(info);
            character.Visible = Visibility;
            if (info.castingName != info.name)
            {
                character.CastingName = info.castingName;
            }
            //�½���ɫ�����ɫ�ֵ���
            AllCharacters.Add(info.name.ToLower(), character);
            if (Visibility == true)
            {
                character.Show(immediate);
            }
            return character;
        }
        /// <summary>
        /// ��ȡ��ɫ��Ϣ
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        private CharacterInfo GetCharacterInfo(string characterName)
        {
            CharacterInfo result = new();
            //�����ַָ������ָ��as
            string[] nameData = characterName.Split(ID_ChracterCasting, System.StringSplitOptions.RemoveEmptyEntries);
            //��ȡ����
            result.name = nameData[0];
            //������ݳ�����ʹ��
            result.castingName = nameData.Length > 1 ? nameData[1] : result.name;
            result.configData = Config.GetCharacterConfigData(result.castingName);
            result.prefab = GetChracterPrefab(result.castingName);
            result.rootCharacterFolder = FormatCharacterPath(ID_CharacterRootPath, result.castingName);
            return result;
        }
        /// <summary>
        /// ��ȡ��ɫԤ�Ƽ�
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        private GameObject GetChracterPrefab(string characterName)
        {
            string prefabPath = FormatCharacterPath(ID_CharacterPrefabPath, characterName);
            return Resources.Load<GameObject>(prefabPath);
        }
        /// <summary>
        /// ͨ����ɫ��Ϣ������ɫ
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
        /// ��ȡ��ɫ������Ϣ
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
        /// ��ɫ�����ȼ�����
        /// </summary>
        public void SortCharacters()
        {
            List<Character> activeCharacters = AllCharacters.Values.Where(c => c.Root.gameObject.activeInHierarchy).ToList(); //&& c.isVisible�����ǿɼ��Ե������ã����ù�?
            List<Character> inactiveCharacters = AllCharacters.Values.Except(activeCharacters).ToList();
            activeCharacters.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            //�����б�
            activeCharacters.Concat(inactiveCharacters);
            SortCharacters(activeCharacters);
        }
        /// <summary>
        /// ��ɫ�����ȼ�����(δ��ɣ�2024/7/18��ep9.2)
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