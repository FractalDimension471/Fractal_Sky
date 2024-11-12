using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace CHARACTERS
{
    /// <summary>
    /// 精灵角色类
    /// </summary>
    public class CharacterSprite : Character
    {
        #region 属性/Property
        //标签字符串（用于防止直接访问物件，方便后续修改，后同）
        private static string ID_SpriteRenderName { get; } = "Renderers";
        //获取根物件上的CanvasGroupe
        private CanvasGroup RootCG => Root.GetComponent<CanvasGroup>();
        //角色图层
        public List<CharacterSpriteLayer> SpriteLayers { get; }
        public override bool Active => Visible && RootCG.alpha == 1;
        public override bool Visible
        {
            get { return IsShowing || RootCG.alpha == 1; }
            set { RootCG.alpha = value ? 1 : 0; }
        }
        //角色美术资产
        private string ArtAssetsDirectory { get; }
        #endregion
        #region 方法/Method
        /// <summary>
        /// 构建精灵角色类
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configData"></param>
        /// <param name="prefab"></param>
        /// <param name="rootAssetsFolder"></param>
        public CharacterSprite(string name, CharacterConfigData configData, GameObject prefab, string rootAssetsFolder) : base(name, configData, prefab)
        {
            SpriteLayers = new();
            RootCG.alpha = 0;
            ArtAssetsDirectory = rootAssetsFolder + "/Images";
            GetLayers();
            Debug.Log($"Create Sprite Character: '{name}'");
        }
        /// <summary>
        /// 设置角色可见性
        /// </summary>
        /// <param name="show"></param>
        /// <returns></returns>
        public override IEnumerator SettingVisibility(bool show, bool immediate)
        {
            float targetAlpha = show ? 1f : 0;
            CanvasGroup self = RootCG;
            while(self.alpha!=targetAlpha)
            {
                if (immediate)
                {
                    self.alpha = targetAlpha;
                }
                else
                {
                    self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, 3f * Time.deltaTime);
                }
                yield return null;
            }
            Co_show = null;
            Co_hide = null;
        }
        private void GetLayers()
        {
            Transform rendererRoot = Animator.transform.Find(ID_SpriteRenderName);
            if (rendererRoot == null) 
            {
                return;
            }
            //传递根物件的控件
            for (int i = 0; i < rendererRoot.transform.childCount; Interlocked.Increment(ref i)) 
            {
                Transform child = rendererRoot.transform.GetChild(i);
                if (child.TryGetComponent<Image>(out var rendererImage))
                {
                    //实例化
                    CharacterSpriteLayer spriteLyaer = new(rendererImage, i);
                    SpriteLayers.Add(spriteLyaer);
                    //重命名
                    child.name = $"Layer:{i}";
                }
                else
                {
                    Debug.LogError("Cannot get layers");
                }
            }
        }
        /// <summary>
        /// 设置精灵图层
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="layer"></param>
        public void SetSprite(Sprite sprite,int layer = 0)
        {
            SpriteLayers[layer].SetSprite(sprite);
        }
        /// <summary>
        /// 获取精灵图层
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public Sprite GetSprite(string spriteName)
        {
            if (ConfigData.Sprites.Count > 0)
            {
                if(ConfigData.Sprites.TryGetValue(spriteName, out Sprite sprite))
                {
                    return sprite;
                }
            }

            if(ConfigData.CharacterType==CharacterType.SpriteSheet)
            {
                //暂无:ep8.1,18:30(start)
                return null;
            }
            else
            {
                return Resources.Load<Sprite>($"{ArtAssetsDirectory}/{spriteName}");
            }
        }
        /// <summary>
        /// 图层过渡变换
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="layer"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public Coroutine TransitionSprite(Sprite sprite, int layer = 0, float speed = 1)
        {
            CharacterSpriteLayer spriteLyaer = SpriteLayers[layer];
            return spriteLyaer.TransitionSprite(sprite,speed);
        }
        /// <summary>
        /// 设置角色颜色
        /// </summary>
        /// <param name="color"></param>
        public override void SetColor(Color color)
        {
            base.SetColor(color);
            color = DisplayColor;
            foreach(CharacterSpriteLayer spriteLayer in SpriteLayers)
            {
                spriteLayer.StopTransitioningColor();
                spriteLayer.SetColor(color);
            }
        }
        /// <summary>
        /// 变换颜色
        /// </summary>
        /// <param name="color"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public override IEnumerator TransitioningColor(Color color, float speed)
        {
            foreach(CharacterSpriteLayer spriteLayer in SpriteLayers)
            {
                spriteLayer.TransitionColor(color, speed);
            }
            yield return null;
            while (SpriteLayers.Any(l => l.IsTransitioningColor))
            {
                yield return null;
            }
            Co_transitioningColor = null;
        }
        /// <summary>
        /// 角色活跃状态
        /// </summary>
        /// <param name="active"></param>
        /// <param name="speedMutiplier"></param>
        /// <returns></returns>
        public override IEnumerator CharacterActivating(bool active, float speedMutiplier, bool immediate = false)
        {
            Color targetColor = DisplayColor;
            foreach(CharacterSpriteLayer spriteLayer in SpriteLayers)
            {
                if (immediate)
                {
                    spriteLayer.SetColor(DisplayColor);
                }
                else
                {
                    spriteLayer.TransitionColor(targetColor, speedMutiplier);
                }
            }
            while (SpriteLayers.Any(l => l.IsTransitioningColor))
            {
                yield return null;
            }
            Co_activating = null;
        }
        /// <summary>
        /// 角色朝向
        /// </summary>
        /// <param name="faceLeft"></param>
        /// <param name="speedMultiplier"></param>
        /// <param name="immediate"></param>
        /// <returns></returns>
        public override IEnumerator FacingDirection(bool faceLeft, float speedMultiplier, bool immediate)
        {
            foreach(CharacterSpriteLayer spriteLayer in SpriteLayers)
            {
                if (faceLeft)
                {
                    spriteLayer.FaceLeft(speedMultiplier, immediate);
                }
                else
                {
                    spriteLayer.FaceRight(speedMultiplier, immediate);
                }
                yield return null;

                while (SpriteLayers.Any(l => l.IsFlipping))
                {
                    yield return null;
                }
                Co_flipping = null;
            }
        }
        public override void OnReceiveCastingExpression(int layer, string expression)
        {
            Sprite sprite = GetSprite(expression);
            if (sprite == null)
            {
                Debug.LogWarning($"Sprite'{expression}'cannot be found.");
                return;
            }
            TransitionSprite(sprite, layer);
        }
        public override void SetVisibility(bool visibility)
        {
            float targetAlpha = visibility ? 1f : 0;
            CanvasGroup self = RootCG;
            while (self.alpha != targetAlpha)
            {
                self.alpha = targetAlpha;
            }
        }
        #endregion
    }
}