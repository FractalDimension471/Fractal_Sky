using CHARACTERS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace COMMANDS
{
    public class CharacterExtension : DatabaseExtention
    {
        #region ����/Property
        private static string[] ID_Immediate { get; } = { "/i", "/immediate" };
        private static string[] ID_Enable { get; } = { "/e", "/enabled" };
        private static string[] ID_Speed { get; } = { "/spd", "/speed" };
        private static string[] ID_Smooth { get; } = { "/s", "/smooth" };
        private static string[] ID_Color { get; } = { "/c", "/color", "/colour" };
        private static string[] ID_Sprite { get; } = { "/s", "/sprite" };
        private static string[] ID_Layer { get; } = { "/l", "/layer" };
        private static string ID_Xposition { get; } = "/x";
        private static string ID_Yposition { get; } = "/y";
        #endregion
        #region ����/Method
        new public static void Extend(CommandDatabase database)
        {
            //��������װΪί��
            database.AddCommand("createcharacter", new Action<string[]>(CreateCharacter));
            database.AddCommand("showcharacters", new Func<string[], IEnumerator>(ShowCharacters));
            database.AddCommand("hidecharacters", new Func<string[], IEnumerator>(HideCharacters));
            //������������ƶ���ɫ
            //database.AddCommand("movecharacters", new Func<string[], IEnumerator>(MoveCharacters));
            //��ָ���������ɫ
            CommandDatabase genericCommands = CommandManager.Instance.CreateSubDatabase(CommandManager.ID_CharacterDB_Generic);
            genericCommands.AddCommand("move", new Func<string[], IEnumerator>(MoveCharacter));
            genericCommands.AddCommand("setpriority", new Action<string[]>(SetCharacterPriority));
            genericCommands.AddCommand("setcolor", new Func<string[], IEnumerator>(SetCharacterColor));
            genericCommands.AddCommand("activate", new Func<string[], IEnumerator>(ActivateCharacter));
            genericCommands.AddCommand("inactivate", new Func<string[], IEnumerator>(InactivateCharacter));
            genericCommands.AddCommand("show", new Func<string[], IEnumerator>(ShowCharacters));
            genericCommands.AddCommand("hide", new Func<string[], IEnumerator>(HideCharacters));

            CommandDatabase spriteCommands = CommandManager.Instance.CreateSubDatabase(CommandManager.ID_CharacterDB_Sprite);
            spriteCommands.AddCommand("setsprite", new Func<string[], IEnumerator>(SetCharacterSprite));


        }
        public static void CreateCharacter(string[] data)
        {
            string characterName = CharacterManager.Instance.GetNameFromAlias(data[0]);
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(ID_Enable, out bool enable, false);
            parameters.TryGetValue(ID_Immediate, out bool immediate, false);
            CharacterManager.Instance.CreateCharacter(characterName, enable, immediate);
        }
        public static IEnumerator ShowCharacters(string[] data)
        {
            List<Character> characters = new();
            foreach (string c in data)
            {
                Character character = CharacterManager.Instance.GetCharacter(CharacterManager.Instance.GetNameFromAlias(c));
                if (character != null)
                {
                    characters.Add(character);
                }
            }
            if (characters.Count == 0)
            {
                yield break;
            }
            //תΪ�����������Ҫ��
            var parameters = ConvertDataToParameters(data);
            //��ȡ�������
            parameters.TryGetValue(ID_Immediate, out bool immediate, false);
            foreach (Character character in characters)
            {
                character.Show(immediate);
            }
            if (!immediate)
            {
                CommandManager.Instance.AddEndingActionToCurrentProcess(() =>
                {
                    foreach (Character character in characters)
                    {
                        character.SetVisibility(true);
                    }
                });
                //�ȴ����
                while (characters.Any(c => c.IsShowing))
                {
                    yield return null;
                }
            }
        }
        public static IEnumerator HideCharacters(string[] data)
        {
            List<Character> characters = new();
            foreach (string c in data)
            {
                Character character = CharacterManager.Instance.GetCharacter(CharacterManager.Instance.GetNameFromAlias(c));
                if (character != null)
                {
                    characters.Add(character);
                }
            }
            if (characters.Count == 0)
            {
                yield break;
            }
            //תΪ�����������Ҫ��
            var parameters = ConvertDataToParameters(data);
            //��ȡ�������
            parameters.TryGetValue(ID_Immediate, out bool immediate, false);
            foreach (Character character in characters)
            {
                character.Hide(immediate);
            }
            if (!immediate)
            {
                CommandManager.Instance.AddEndingActionToCurrentProcess(() =>
                {
                    foreach (Character character in characters)
                    {
                        character.SetVisibility(false);
                    }
                });
                //�ȴ����
                while (characters.Any(c => c.IsShowing))
                {
                    yield return null;
                }
            }
        }
        /// <summary>
        /// �ƶ���ɫ
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerator MoveCharacter(string[] data)
        {
            string characterName = CharacterManager.Instance.GetNameFromAlias(data[0]);
            Character character = CharacterManager.Instance.GetCharacter(characterName);
            if (character == null)
            {
                yield break;
            }
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(ID_Xposition, out float x);
            parameters.TryGetValue(ID_Yposition, out float y);
            parameters.TryGetValue(ID_Speed, out float speed, 1);
            parameters.TryGetValue(ID_Smooth, out bool smooth, false);
            parameters.TryGetValue(ID_Immediate, out bool immediate, false);
            Vector2 position = new Vector2(x, y);
            if (immediate)
            {
                character.SetPosition(position);
            }
            else
            {
                CommandManager.Instance.AddEndingActionToCurrentProcess(() => { character?.SetPosition(position); });
                yield return character.MoveToPosition(position, speed, smooth);
            }
        }
        private static void SetCharacterPriority(string[] data)
        {
            string characterName = CharacterManager.Instance.GetNameFromAlias(data[0]);
            Character character = CharacterManager.Instance.GetCharacter(characterName);
            if (character == null || data.Length < 2)
            {
                return;
            }
            int priority = int.Parse(data[1]);
            character.SetPriority(priority);
            CharacterManager.Instance.SortCharacters();
        }
        private static IEnumerator SetCharacterColor(string[] data)
        {
            bool immediate;
            Color color = Color.white;
            string characterName = CharacterManager.Instance.GetNameFromAlias(data[0]);
            Character character = CharacterManager.Instance.GetCharacter(characterName);
            if (character == null || data.Length < 2)
            {
                yield break;
            }
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(ID_Color, out string colorName);
            if (!parameters.TryGetValue(ID_Speed, out float speed, 1f))
            {
                parameters.TryGetValue(ID_Immediate, out immediate, true);
            }
            else
            {
                immediate = false;
            }
            color = color.GetColorByName(colorName);
            if (immediate)
            {
                character.SetColor(color);
            }
            else
            {
                CommandManager.Instance.AddEndingActionToCurrentProcess(() => { character?.SetColor(color); });
                yield return character.TransitionColor(color, speed);
            }
        }
        private static IEnumerator ActivateCharacter(string[] data)
        {
            string characterName = CharacterManager.Instance.GetNameFromAlias(data[0]);
            Character character = CharacterManager.Instance.GetCharacter(characterName);
            if (character == null || data.Length < 2)
            {
                yield break;
            }
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(ID_Immediate, out bool immediate, false);
            if (immediate)
            {
                character.Activate(immediate: true);
            }
            else
            {
                CommandManager.Instance.AddEndingActionToCurrentProcess(() => { character?.Activate(immediate: true); });
                yield return character.Activate();
            }
        }
        private static IEnumerator InactivateCharacter(string[] data)
        {
            string characterName = CharacterManager.Instance.GetNameFromAlias(data[0]);
            Character character = CharacterManager.Instance.GetCharacter(characterName);
            if (character == null || data.Length < 2)
            {
                yield break;
            }
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(ID_Immediate, out bool immediate, false);
            if (immediate)
            {
                character.Inactivate(immediate: true);
            }
            else
            {
                CommandManager.Instance.AddEndingActionToCurrentProcess(() => { character?.Inactivate(immediate: true); });
                yield return character.Inactivate();
            }
        }
        private static IEnumerator SetCharacterSprite(string[] data)
        {
            bool immediate = false;
            string characterName = CharacterManager.Instance.GetNameFromAlias(data[0]);
            CharacterSprite characterSprite = CharacterManager.Instance.GetCharacter(characterName) as CharacterSprite;
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(ID_Sprite, out string spriteName);
            parameters.TryGetValue(ID_Layer, out int layer, 0);
            if (!parameters.TryGetValue(ID_Speed, out float speed, 0.1f))
            {
                parameters.TryGetValue(ID_Immediate, out immediate, true);
            }
            Sprite sprite = characterSprite.GetSprite(spriteName);
            if (sprite == null)
            {
                yield break;
            }
            if (immediate)
            {
                characterSprite.SetSprite(sprite, layer);
            }
            else
            {
                CommandManager.Instance.AddEndingActionToCurrentProcess(() => { characterSprite.SetSprite(sprite, layer); });
                yield return characterSprite.TransitionSprite(sprite, layer, speed);
            }
        }
        #endregion
    }
}