using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;
using TMPro;
namespace CHARACTERS
{
    /// <summary>
    /// 角色基本类
    /// </summary>
    public abstract class Character
    {

        #region 属性

        public string Name { get; } = "";
        public string DisplayName { get; internal set; } = "";
        public string CastingName { get; internal set; } = "";
        public int Priority { get; protected set; }
        public virtual bool Visible { get; set; } = false;
        public virtual bool Active { get; set; } = false;
        public Vector2 Position { get; set; }
        public enum CharacterType { Text, Sprite, SpriteSheet, Live2D, Model3D }

        private static float C_DarkDegree { get; } = 0.5f;
        public static string ID_RefreshTrigger { get; } = "Refresh";

        public RectTransform Root { get; } = null;
        public CharacterConfigData ConfigData { get; private set; }
        public Animator Animator { get; }

        public Color Color { get; protected set; } = Color.white;
        protected Color DisplayColor => CharacterActived ? ActiveColor : InactiveColor;
        protected Color ActiveColor => Color;
        protected Color InactiveColor => new(Color.r*C_DarkDegree, Color.g*C_DarkDegree, Color.b*C_DarkDegree, Color.a);

        protected Coroutine Co_show { get; set; }
        protected Coroutine Co_hide {  get; set; }
        protected Coroutine Co_moving { get; set; }
        protected Coroutine Co_transitioningColor { get; set; }
        protected Coroutine Co_activating { get; set; }
        protected Coroutine Co_flipping {  get; set; }

        public static bool DefaultOrientation_L { get; } = true;
        private bool FacingLeft { get; set; } = DefaultOrientation_L;
        private bool CharacterActived { get; set; } = true;

        public bool IsShowing => Co_show != null;
        public bool IsHiding => Co_hide != null;
        public bool IsMoving => Co_moving != null;
        public bool IsTransitioningColor => Co_transitioningColor != null;
        public bool IsActive => (CharacterActived && Co_activating != null);
        public bool IsInactive => (!CharacterActived && Co_activating != null);
        public bool IsFlipping => Co_flipping != null;
        public bool IsFacingLeft => FacingLeft;

        private CharacterManager CharacterManager => CharacterManager.Instance;
        private DialogueSystem DialogueSystem => DialogueSystem.Instance;
        #endregion
        #region 方法
        /// <summary>
        /// 构建通用角色
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configData"></param>
        /// <param name="prefab"></param>
        public Character(string name, CharacterConfigData configData, GameObject prefab)
        {
            Name = name;
            DisplayName = name;
            ConfigData = configData;

            //预制件非空直接实例化
            if (prefab != null)
            {
                GameObject ob = Object.Instantiate(prefab, CharacterManager.CharacterPanel);
                ob.name = CharacterManager.FormatCharacterPath(CharacterManager.ID_CharacterPrefabName, name);
                ob.SetActive(true);
                Root = ob.GetComponent<RectTransform>();
                Animator = Root.GetComponentInChildren<Animator>();
            }
            SetPosition(new Vector2 (0.5f, 0.5f));
        }
        /// <summary>
        /// 角色说话
        /// </summary>
        /// <param name="dialogue"></param>
        /// <returns></returns>
        public Coroutine Say(string dialogue) => Say(new List<string>{dialogue});
        /// <summary>
        /// 角色说话
        /// </summary>
        /// <param name="dialogue"></param>
        /// <returns></returns>
        public Coroutine Say(List<string> dialogue)
        {
            UpdateCostomlizedSetting();
            DialogueSystem.ShowSpeakerName(DisplayName);
            return DialogueSystem.Say(dialogue);
        }
        /// <summary>
        /// 设置角色可见性
        /// </summary>
        /// <param name="visibility"></param>
        public virtual void SetVisibility(bool visibility)
        {
            Debug.LogError("This is the base of all characters!");
        }
        /// <summary>
        /// 设置角色名称颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetNameColor(Color color) => ConfigData.NameColor = color;
        /// <summary>
        /// 设置角色名称字体
        /// </summary>
        /// <param name="font"></param>
        public void SetNameFont(TMP_FontAsset font) => ConfigData.NameFont = font;
        /// <summary>
        /// 设置角色对话颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetDialogueColor(Color color) => ConfigData.DialogueColor = color;
        /// <summary>
        /// 设置角色对话字体
        /// </summary>
        /// <param name="font"></param>
        public void SetDialogueFont(TMP_FontAsset font) => ConfigData.DialogueFont = font;
        /// <summary>
        /// 更新自定义设置
        /// </summary>
        public void UpdateCostomlizedSetting() => DialogueSystem.ApplySpeakerData(ConfigData);
        /// <summary>
        /// 还原默认设置
        /// </summary>
        public void ResetSettings() => ConfigData = CharacterManager.Instance.GetCharacterConfigData(Name, true);
        /// <summary>
        /// 显示角色
        /// </summary>
        /// <returns></returns>
        public virtual Coroutine Show(bool immediate = false)
        {
            if (IsShowing)
            {
                CharacterManager.StopCoroutine(Co_show);
            }
            if (IsHiding) 
            {
                CharacterManager.StopCoroutine(Co_hide);
            }
            Co_show = CharacterManager.StartCoroutine(SettingVisibility(true, immediate));
            return Co_show;
        }/// <summary>
        /// 隐藏角色
        /// </summary>
        /// <returns></returns>
        public virtual Coroutine Hide(bool immediate = false)
        {
            if(IsHiding)
            {
                CharacterManager.StopCoroutine(Co_hide);
            }
            if(IsShowing)
            {
                CharacterManager.StopCoroutine(Co_show);
            }
            Co_hide = CharacterManager.StartCoroutine(SettingVisibility(false, immediate));
            return Co_hide;
        }
        public virtual IEnumerator SettingVisibility(bool show, bool immediate)
        {
            Debug.Log("This is the base of all characters!");
            return null;
        }
        /// <summary>
        /// 设定角色位置
        /// </summary>
        /// <param name="position"></param>
        public virtual void SetPosition(Vector2 position)
        {
            if(Root == null)
            {
                return;
            }
            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertPosition(position);
            Root.anchorMin = minAnchorTarget;
            Root.anchorMax = maxAnchorTarget;
            //记录当前位置
            Position = position;
        }
        /// <summary>
        /// 角色移动至指定位置
        /// </summary>
        /// <param name="position"></param>
        /// <param name="speed"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public virtual Coroutine MoveToPosition(Vector2 position,float speed=2f,bool smooth=false)
        {
            if(Root==null)
            {
                return null;
            }
            if (IsMoving)
            {
                CharacterManager.StopCoroutine(Co_moving);
            }
            Co_moving = CharacterManager.StartCoroutine(MovingToPosition(position,speed,smooth));
            //记录当前位置
            Position = position;
            return Co_moving;
        }
        private IEnumerator MovingToPosition(Vector2 position, float speed = 2f, bool smooth = false)
        {
            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertPosition(position);
            Vector2 padding = Root.anchorMax - Root.anchorMin;
            while(Root.anchorMin!=minAnchorTarget||Root.anchorMax!=maxAnchorTarget)
            {
                if(smooth)
                {
                    Root.anchorMin = Vector2.Lerp(Root.anchorMin, minAnchorTarget, speed * Time.deltaTime);
                }
                else
                {
                    Root.anchorMin= Vector2.MoveTowards(Root.anchorMin, minAnchorTarget, speed * Time.deltaTime*0.35f);
                }
                Root.anchorMax = minAnchorTarget + padding;
                //防止动画过长，达到某个程度直接瞬移
                if(smooth&&Vector2.Distance(Root.anchorMin,minAnchorTarget)<=0.001f)
                {
                    Root.anchorMin = minAnchorTarget;
                    Root.anchorMax = maxAnchorTarget;
                    break;
                }
                yield return null;
            }
            Co_moving = null;
        }
        protected (Vector2, Vector2) ConvertPosition(Vector2 position)
        {
            Vector2 padding = Root.anchorMax - Root.anchorMin;
            float maxX = 1f - padding.x;
            float maxY = 1f - padding.y;
            Vector2 minAnchor = new(maxX * position.x, maxY * position.y);
            Vector2 maxAnchor = minAnchor + padding;
            return (minAnchor, maxAnchor);
        }
        /// <summary>
        /// 设置角色颜色
        /// </summary>
        /// <param name="color"></param>
        public virtual void SetColor(Color color)
        {
            this.Color = color;
        }
        /// <summary>
        /// 变换角色颜色
        /// </summary>
        /// <param name="color"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public Coroutine TransitionColor(Color color,float speed=1f)
        {
            this.Color = color;
            if (IsTransitioningColor)
            {
                CharacterManager.StopCoroutine(Co_transitioningColor);
            }
            Co_transitioningColor = CharacterManager.StartCoroutine(TransitioningColor(DisplayColor, speed));
            return Co_transitioningColor;
        }  
        public virtual IEnumerator TransitioningColor(Color color,float speed)
        {
            Debug.Log("This is the base of all characters!");
            return null;
        }
        /// <summary>
        /// 角色设置为活跃状态
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public Coroutine Activate(float speed=1f, bool immediate=false)
        {
            if (IsActive || IsInactive) 
            {
                CharacterManager.StopCoroutine(Co_activating);
            }
            
            CharacterActived = true;
            Co_activating = CharacterManager.StartCoroutine(CharacterActivating(CharacterActived,speed,immediate));
            return Co_activating;
        }
        /// <summary>
        /// 角色设置为非活跃状态
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public Coroutine Inactivate(float speed = 1f, bool immediate = false)
        {
            if (IsInactive || IsActive) 
            {
                CharacterManager.StopCoroutine(Co_activating);  
            }
            CharacterActived = false;
            Co_activating = CharacterManager.StartCoroutine(CharacterActivating(CharacterActived,speed,immediate));
            return Co_activating;
        }
        public virtual IEnumerator CharacterActivating(bool active, float speedMutiplier, bool immediate = false)
        {
            Debug.Log("This is the base of all characters!");
            return null;
        }
        /// <summary>
        /// 角色水平翻转
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="immediate"></param>
        /// <returns></returns>
        public Coroutine Flip(float speed = 1, bool immediate = false)
        {
            if (IsFacingLeft)
            {
                return FaceRight(speed,immediate);
            }
            else
            {
                return FaceLeft(speed,immediate);
            }
        }
        /// <summary>
        /// 角色面向左侧
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="immediate"></param>
        /// <returns></returns>
        public Coroutine FaceLeft(float speed = 1, bool immediate = false)
        {
            if (IsFlipping)
            {
                CharacterManager.StopCoroutine(Co_flipping);
            }
            FacingLeft = true;
            Co_flipping = CharacterManager.StartCoroutine(FacingDirection(FacingLeft, speed, immediate));
            return Co_flipping;
        }
        /// <summary>
        /// 角色面向右侧
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="immediate"></param>
        /// <returns></returns>
        public Coroutine FaceRight(float speed = 1, bool immediate = false)
        {
            if (IsFlipping)
            {
                CharacterManager.StopCoroutine(Co_flipping);
            }
            FacingLeft = false;
            Co_flipping = CharacterManager.StartCoroutine(FacingDirection(FacingLeft, speed, immediate));
            return Co_flipping;
        }
        public virtual IEnumerator FacingDirection(bool faceLeft,float speedMultiplier,bool immediate)
        {
            Debug.Log("This is the base of all characters!");
            return null;
        }
        /// <summary>
        /// 设置角色显示优先级
        /// </summary>
        /// <param name="priority"></param>
        public void SetPriority(int priority, bool autoSortCharactersOnUI = true)
        {
            Priority = priority; 
            if (autoSortCharactersOnUI)
            {
                CharacterManager.SortCharacters();
            }
        }
        /// <summary>
        /// 进行动画
        /// </summary>
        /// <param name="animation"></param>
        public void Animate(string animation)
        {
            Animator.SetTrigger(animation);
        }
        public void Animate(string animation, bool state)
        {
            Animator.SetTrigger(ID_RefreshTrigger);
            Animator.SetBool(animation, state);
        }
        public virtual void OnReceiveCastingExpression(int layer,string expression)
        {
            return;
        }
        #endregion
    }
}