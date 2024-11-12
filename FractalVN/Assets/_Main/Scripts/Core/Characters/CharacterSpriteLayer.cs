using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace CHARACTERS
{
    /// <summary>
    /// 精灵角色图层类
    /// </summary>
    public class CharacterSpriteLayer
    {
        #region 属性/Property
        private float DefaultSpeed => DialogueSystem.Instance.TransitionSpeed;
        private float TransitionSpeedMultiplier { get; set; }
        
        public int Layer { get; } = 0;
        public Image Renderer { get; private set; }
        public CanvasGroup RendererCG => Renderer.GetComponent<CanvasGroup>();
        private CharacterManager CharacterManager => CharacterManager.Instance;
        private List<CanvasGroup> OldRenderers { get; } = new();
        //跟踪协程
        private Coroutine Co_transitioningLayer { get; set; } = null;
        private Coroutine Co_levelingAlpha { get; set; } = null;
        private Coroutine Co_transitioningColor { get; set; }
        private Coroutine Co_flipping { get; set; } = null;

        public bool IsTransitioningLayer => Co_transitioningLayer != null;
        public bool IsLevelingAlpha => Co_levelingAlpha != null;
        public bool IsTransitioningColor => Co_transitioningColor != null;
        public bool IsFlipping => Co_flipping != null;
        private bool FacingLeft { get; set; } = Character.DefaultOrientation_L;
        #endregion
        #region 方法/Method
        /// <summary>
        /// 构建精灵角色图层
        /// </summary>
        /// <param name="defaultRenderer"></param>
        /// <param name="layer"></param>
        public CharacterSpriteLayer(Image defaultRenderer, int layer = 0)
        {
            TransitionSpeedMultiplier = 1;

            Layer = layer;
            Renderer = defaultRenderer;
        }
        /// <summary>
        /// 设置精灵图层
        /// </summary>
        /// <param name="sprite"></param>
        public void SetSprite(Sprite sprite)
        {
            Renderer.sprite = sprite;
        }
        /// <summary>
        /// 变换精灵图层
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public Coroutine TransitionSprite(Sprite sprite, float speed = 1)
        {
            if(sprite==Renderer.sprite)
            {
                return null;
            }
            if(IsTransitioningLayer)
            {
                CharacterManager.StopCoroutine(Co_transitioningLayer);
            }
            Co_transitioningLayer = CharacterManager.StartCoroutine(TransitioningSprite(sprite, speed));
            return Co_transitioningLayer;
        }
        public IEnumerator TransitioningSprite(Sprite sprite, float speedMultiplier = 1)
        {
            TransitionSpeedMultiplier = speedMultiplier;
            Image newRenderer = CreateRenderer(Renderer.transform.parent);
            newRenderer.sprite = sprite;
            yield return TryStarLevelingAlphas();
            Co_transitioningLayer = null;
        }
        /// <summary>
        /// 创建渲染层
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private Image CreateRenderer(Transform parent)
        {
            Image newRenderer = Object.Instantiate(Renderer,parent);
            OldRenderers.Add(RendererCG);
            newRenderer.name = Renderer.name;
            Renderer = newRenderer;
            Renderer.gameObject.SetActive(true);
            RendererCG.alpha = 0;
            return newRenderer;
        }
        private Coroutine TryStarLevelingAlphas()
        {
            if(IsLevelingAlpha)
            {
                CharacterManager.StopCoroutine(Co_levelingAlpha);
            }
            Co_levelingAlpha = CharacterManager.StartCoroutine(RunAlphaLeveling());
            return Co_levelingAlpha;
        }
        private IEnumerator RunAlphaLeveling()
        {
            while (RendererCG.alpha < 1 || OldRenderers.Any(oldCG => oldCG.alpha > 0)) 
            {
                float levelingspeed = DefaultSpeed * TransitionSpeedMultiplier * Time.deltaTime;
                RendererCG.alpha = Mathf.MoveTowards(RendererCG.alpha, 1, levelingspeed);
                for(int i=OldRenderers.Count-1;i>=0;i--)
                {
                    CanvasGroup oldCG = OldRenderers[i];
                    oldCG.alpha = Mathf.MoveTowards(oldCG.alpha, 0, levelingspeed);
                    if (oldCG.alpha <= 0)
                    {
                        OldRenderers.RemoveAt(i);
                        Object.Destroy(oldCG.gameObject);
                    }
                }
                yield return null;
            }
            Co_levelingAlpha = null;
        }
        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color)
        {
            Renderer.color = color;
            foreach(CanvasGroup oldCG in OldRenderers)
            {
                oldCG.GetComponent<Image>().color = color;
            }
        }
        /// <summary>
        /// 变换颜色
        /// </summary>
        /// <param name="color"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public Coroutine TransitionColor(Color color, float speed)
        {
            if (IsTransitioningColor)
            {
                CharacterManager.StopCoroutine(Co_transitioningColor);
            }
            Co_transitioningColor = CharacterManager.StartCoroutine(TransitioningColor(color, speed));
            return Co_transitioningColor;
        }
        /// <summary>
        /// 停止变换颜色
        /// </summary>
        public void StopTransitioningColor()
        {
            if (!IsTransitioningColor)
            {
                return;
            }
            CharacterManager.StopCoroutine(Co_transitioningColor);
            Co_transitioningColor = null;
        }
        private IEnumerator TransitioningColor(Color color, float speedMultiplier)
        {
            Color oldColor = Renderer.color;
            List<Image> oldImages = new List<Image>();
            foreach(CanvasGroup oldCG in OldRenderers)
            {
                oldImages.Add(oldCG.GetComponent<Image>());
            }
            float colorPercent = 0;
            while (colorPercent < 1)
            {
                colorPercent += DefaultSpeed * speedMultiplier * Time.deltaTime;
                Renderer.color = Color.Lerp(oldColor, color, colorPercent);
                foreach(Image oldImage in oldImages)
                {
                    if(oldImage != null)
                    {
                        oldImage.color = Renderer.color;
                    }
                    else
                    {
                        oldImages.Remove(oldImage);
                    }
                }
                yield return null;
            }
            Co_transitioningColor = null;
        }
        /// <summary>
        /// 翻转图层
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="immediate"></param>
        /// <returns></returns>
        public Coroutine Flip(float speed = 1, bool immediate = false)
        {
            if (FacingLeft)
            {
                return FaceRight(speed, immediate);
            }
            else
            {
                return FaceLeft(speed, immediate);
            }
        }
        /// <summary>
        /// 图层朝左
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="immediate"></param>
        /// <returns></returns>
        public Coroutine FaceLeft(float speed = 1, bool immediate = false)
        {
            if (Co_flipping != null)
            {
                CharacterManager.StopCoroutine(Co_flipping);
            }
            FacingLeft = true;
            Co_flipping = CharacterManager.StartCoroutine(FacingDirection(FacingLeft, speed, immediate));
            return Co_flipping;
        }
        /// <summary>
        /// 图层朝右
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="immediate"></param>
        /// <returns></returns>
        public Coroutine FaceRight(float speed = 1, bool immediate = false)
        {
            if (Co_flipping != null)
            {
                CharacterManager.StopCoroutine(Co_flipping);
            }
            FacingLeft = false;
            Co_flipping = CharacterManager.StartCoroutine(FacingDirection(FacingLeft, speed, immediate));
            return Co_flipping;
        }
        private IEnumerator FacingDirection(bool faceLeft, float speedMultiplier, bool immediate)
        {
            float xScale = faceLeft ? 1 : -1;
            Vector3 newScale= new Vector3(xScale, 1, 1);
            if (!immediate)
            {
                Image newRenderer = CreateRenderer(Renderer.transform.parent);
                newRenderer.transform.localScale= newScale;
                TransitionSpeedMultiplier = speedMultiplier;
                TryStarLevelingAlphas();
                //等待完成
                while (IsLevelingAlpha)
                {
                    yield return null;
                }
            }
            else
            {
                Renderer.transform.localScale = newScale;
            }
            Co_flipping = null;
        }
        #endregion
    }
}