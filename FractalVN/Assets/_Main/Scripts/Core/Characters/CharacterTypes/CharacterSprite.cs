using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace CHARACTERS
{
    /// <summary>
    /// �����ɫ��
    /// </summary>
    public class CharacterSprite : Character
    {
        #region ����/Property
        //��ǩ�ַ��������ڷ�ֱֹ�ӷ����������������޸ģ���ͬ��
        private static string ID_SpriteRenderName { get; } = "Renderers";
        //��ȡ������ϵ�CanvasGroupe
        private CanvasGroup RootCG => Root.GetComponent<CanvasGroup>();
        //��ɫͼ��
        public List<CharacterSpriteLayer> SpriteLayers { get; }
        public override bool Active => Visible && RootCG.alpha == 1;
        public override bool Visible
        {
            get { return IsShowing || RootCG.alpha == 1; }
            set { RootCG.alpha = value ? 1 : 0; }
        }
        //��ɫ�����ʲ�
        private string ArtAssetsDirectory { get; }
        #endregion
        #region ����/Method
        /// <summary>
        /// ���������ɫ��
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
        /// ���ý�ɫ�ɼ���
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
            //���ݸ�����Ŀؼ�
            for (int i = 0; i < rendererRoot.transform.childCount; Interlocked.Increment(ref i)) 
            {
                Transform child = rendererRoot.transform.GetChild(i);
                if (child.TryGetComponent<Image>(out var rendererImage))
                {
                    //ʵ����
                    CharacterSpriteLayer spriteLyaer = new(rendererImage, i);
                    SpriteLayers.Add(spriteLyaer);
                    //������
                    child.name = $"Layer:{i}";
                }
                else
                {
                    Debug.LogError("Cannot get layers");
                }
            }
        }
        /// <summary>
        /// ���þ���ͼ��
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="layer"></param>
        public void SetSprite(Sprite sprite,int layer = 0)
        {
            SpriteLayers[layer].SetSprite(sprite);
        }
        /// <summary>
        /// ��ȡ����ͼ��
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
                //����:ep8.1,18:30(start)
                return null;
            }
            else
            {
                return Resources.Load<Sprite>($"{ArtAssetsDirectory}/{spriteName}");
            }
        }
        /// <summary>
        /// ͼ����ɱ任
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
        /// ���ý�ɫ��ɫ
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
        /// �任��ɫ
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
        /// ��ɫ��Ծ״̬
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
        /// ��ɫ����
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