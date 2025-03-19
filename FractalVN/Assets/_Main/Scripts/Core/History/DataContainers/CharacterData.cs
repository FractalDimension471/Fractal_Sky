using CHARACTERS;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace HISTORY
{
    [System.Serializable]
    public class CharacterData
    {
        #region 属性/Property
        [field: SerializeField]
        public string CharacterName { get; set; } = "";
        [field: SerializeField]
        public string DisplayName { get; set; } = "";
        [field: SerializeField]
        public string CastingName { get; set; } = "";
        [field: SerializeField]
        public bool Visible { get; set; }
        [field: SerializeField]
        public bool Active { get; set; }
        [field: SerializeField]
        public bool FacingLeft { get; set; }
        [field: SerializeField]
        public Color Color { get; set; }
        [field: SerializeField]
        public int Priority { get; set; }
        [field: SerializeField]
        public Vector2 Position { get; set; }
        [field: SerializeField]
        public string AnimeJSON { get; set; }
        [field: SerializeField]
        public string DataJSON { get; set; }
        [field: SerializeField]
        public CharacterConfiguration Configuration { get; set; }

        [System.Serializable]
        public class CharacterConfiguration
        {
            [field: SerializeField]
            public string Name { get; set; }
            [field: SerializeField]
            public string Alias { get; set; }
            [field: SerializeField]
            public Character.CharacterType CharacterType { get; set; }
            [field: SerializeField]
            public Color NameColor { get; set; }
            [field: SerializeField]
            public Color DialogueColor { get; set; }
            [field: SerializeField]
            public string NameFont { get; set; }
            [field: SerializeField]
            public string DialogueFont { get; set; }
            [field: SerializeField]
            public float NameFontSize { get; set; }
            [field: SerializeField]
            public float DialogueFontSize { get; set; }
        }
        [System.Serializable]
        public class SpriteData
        {
            [field: SerializeField]
            public List<LayerData> LayerDatas { get; set; }

            [System.Serializable]
            public class LayerData
            {
                [field: SerializeField]
                public string Name { get; set; }
                public Color color;//循环引用问题
            }
        }
        [Serializable]
        public class AnimeData
        {
            [field: SerializeField]
            public List<Parameter> Parameters { get; internal set; }
            [Serializable]
            public class Parameter
            {
                [field: SerializeField]
                public string Name { get; set; }
                [field: SerializeField]
                public string Type { get; set; }
                [field: SerializeField]
                public string Value { get; set; }
            }
        }
        /// <summary>
        /// 未实现：ep:22.2
        /// </summary>
        public class Live2DData
        {

        }
        /// <summary>
        /// 未实现：ep:22.2
        /// </summary>
        public class Model3DData
        {

        }
        #endregion
        #region 方法/Method
        public CharacterData(CharacterConfigData reference)
        {
            Configuration = new()
            {
                Name = reference.Name,
                Alias = reference.Alias,
                CharacterType = reference.CharacterType,
                NameColor = reference.NameColor,
                DialogueColor = reference.DialogueColor,
                NameFont = FilePaths.GetPath(FilePaths.DefaultFontPaths, reference.NameFont.name),
                DialogueFont = FilePaths.GetPath(FilePaths.DefaultFontPaths, reference.DialogueFont.name),
                NameFontSize = reference.NameFontSize,
                DialogueFontSize = reference.DialogueFontSize,
            };
        }
        public static List<CharacterData> Capture()
        {
            List<CharacterData> datas = new();
            foreach (var c in CharacterManager.Instance.AllCharacters)
            {
                Character character = c.Value;
                if (!character.Visible)
                {
                    continue;
                }
                CharacterData data = new(character.ConfigData)
                {
                    CharacterName = character.Name,
                    DisplayName = character.DisplayName,
                    CastingName = character.CastingName,
                    Visible = character.Visible,
                    Active = character.Active,
                    FacingLeft = character.IsFacingLeft,
                    Color = character.Color,
                    Priority = character.Priority,
                    Position = character.Position,
                    AnimeJSON = GetAnimeData(character),
                };

                switch (character.ConfigData.CharacterType)
                {
                    case Character.CharacterType.Sprite:
                    case Character.CharacterType.SpriteSheet:
                        data.DataJSON = JsonUtility.ToJson(GetSpriteData(character));
                        //data.DataJSON = JsonConvert.SerializeObject(GetSpriteData(character), FileManager.SerializeSettings);
                        break;
                    case Character.CharacterType.Live2D:
                        //data.DataJSON = JsonConvert.SerializeObject(GetLive2DData(character), FileManager.SerializeSettings);
                        break;
                    case Character.CharacterType.Model3D:
                        //data.DataJSON = JsonConvert.SerializeObject(GetModel3DData(character), FileManager.SerializeSettings);
                        break;
                }
                datas.Add(data);
            }
            return datas;
        }
        public static void Apply(List<CharacterData> datas)
        {
            List<string> cache = new();
            foreach (var data in datas)
            {
                Character character = null;

                if (data.CastingName == string.Empty)
                {
                    character = CharacterManager.Instance.GetCharacter(data.CharacterName, true);
                }
                else
                {
                    character = CharacterManager.Instance.GetCharacter(data.CharacterName, false);
                    if (character == null)
                    {
                        string fullCastingName = $"{data.CharacterName}{CharacterManager.ID_ChracterCasting}{data.CastingName}";
                        character = CharacterManager.Instance.CreateCharacter(fullCastingName);
                    }
                }
                character.DisplayName = data.CharacterName;
                character.SetVisibility(data.Visible);
                character.SetColor(data.Color);
                character.SetPriority(data.Priority);
                character.SetPosition(data.Position);
                SetAnimeData(character, JsonUtility.FromJson<AnimeData>(data.AnimeJSON));
                //SetAnimeData(character,JsonConvert.DeserializeObject<AnimeData>(data.DataJSON));

                if (data.Active)
                {
                    character.Activate(immediate: true);
                }
                else
                {
                    character.Inactivate(immediate: true);
                }
                if (data.FacingLeft)
                {
                    character.FaceLeft(immediate: true);
                }
                else
                {
                    character.FaceRight(immediate: true);
                }
                switch (character.ConfigData.CharacterType)
                {
                    case Character.CharacterType.Sprite:
                    case Character.CharacterType.SpriteSheet:
                        SpriteData spriteData = JsonUtility.FromJson<SpriteData>(data.DataJSON);
                        //SpriteData spriteData = JsonConvert.DeserializeObject<SpriteData>(data.DataJSON);
                        CharacterSprite characterSprite = character as CharacterSprite;
                        SetSpriteData(characterSprite, spriteData);
                        break;
                    case Character.CharacterType.Live2D:
                        //SetLive2DData(),Ep.22.3
                        break;
                    case Character.CharacterType.Model3D:
                        //SetModel3DData()
                        break;
                }
                cache.Add(character.Name);
            }
            foreach (var c in CharacterManager.Instance.AllCharacters)
            {
                Character character = c.Value;
                if (!cache.Contains(character.Name))
                {
                    character.Visible = false;
                }
            }
        }
        private static void SetAnimeData(Character character, AnimeData data)
        {
            Animator animator = character.Animator;
            foreach (var parameter in data.Parameters)
            {
                switch (parameter.Type)
                {
                    case "Bool":
                        if (bool.TryParse(parameter.Value, out bool boolValue))
                        {
                            animator.SetBool(parameter.Name, boolValue);
                        }
                        else
                        {
                            Debug.LogWarning($"Invalid animator parameter type: '{parameter.Value}'");
                        }
                        break;
                    case "Float":
                        if (float.TryParse(parameter.Value, out float floatValue))
                        {
                            animator.SetFloat(parameter.Name, floatValue);
                        }
                        else
                        {
                            Debug.LogWarning($"Invalid animator parameter type: '{parameter.Value}'");
                        }
                        break;
                    case "Int":
                        if (int.TryParse(parameter.Value, out int intValue))
                        {
                            animator.SetInteger(parameter.Name, intValue);
                        }
                        else
                        {
                            Debug.LogWarning($"Invalid animator parameter type: '{parameter.Value}'");
                        }
                        break;
                }
            }
            animator.SetTrigger(Character.ID_RefreshTrigger);
        }
        private static void SetSpriteData(CharacterSprite sc, SpriteData sd)
        {
            for (int i = 0; i < sd.LayerDatas.Count; Interlocked.Increment(ref i))
            {
                var layerData = sd.LayerDatas[i];
                if (sc.SpriteLayers[i].Renderer.sprite != null && sc.SpriteLayers[i].Renderer.sprite.name != layerData.Name)
                {
                    Sprite sprite = sc.GetSprite(layerData.Name);
                    if (sprite != null)
                    {
                        sc.SetSprite(sprite, i);
                    }
                    else
                    {
                        Debug.LogWarning($"Cannot load sprite '{layerData.Name}'");
                    }
                }
            }
        }
        private static void SetLive2DData(CharacterLive2D c)
        {

        }
        private static void SetModel3DData(CharacterModel3D c)
        {

        }
        private static string GetAnimeData(Character character)
        {
            Animator animator = character.Animator;
            AnimeData data = new()
            {
                Parameters = new()
            };
            foreach (var parameter in data.Parameters)
            {
                if (parameter.Type == "Trigger")
                {
                    continue;
                }
                AnimeData.Parameter animeParameter = new()
                {
                    Name = parameter.Name
                };
                switch (parameter.Type)
                {
                    case "Bool":
                        animeParameter.Type = "Bool";
                        animeParameter.Value = animator.GetBool(parameter.Name).ToString();
                        break;
                    case "Float":
                        animeParameter.Type = "Float";
                        animeParameter.Value = animator.GetFloat(parameter.Name).ToString();
                        break;
                    case "Int":
                        animeParameter.Type = "Int";
                        animeParameter.Value = animator.GetInteger(parameter.Name).ToString();
                        break;
                }
                data.Parameters.Add(animeParameter);
            }
            return JsonUtility.ToJson(data);
            //return JsonConvert.SerializeObject(data, FileManager.SerializeSettings);
        }
        private static SpriteData GetSpriteData(Character character)
        {
            SpriteData spriteData = new()
            {
                LayerDatas = new()
            };
            CharacterSprite cs = character as CharacterSprite;
            foreach (var layer in cs.SpriteLayers)
            {
                SpriteData.LayerData layerData = new()
                {
                    Name = layer.Renderer.sprite.name,
                    color = layer.Renderer.color,
                };
                spriteData.LayerDatas.Add(layerData);
            }
            return spriteData;
        }
        private static Live2DData GetLive2DData(Character character)
        {
            return new Live2DData();
        }
        private static Model3DData GetModel3DData(Character character)
        {
            return new Model3DData();
        }
        #endregion
    }
}