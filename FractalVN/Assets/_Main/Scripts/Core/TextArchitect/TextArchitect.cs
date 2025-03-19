using DIALOGUE;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
//定义文本构建器
[Serializable]
public class TextArchitect
{
    #region 属性/Property
    private TextMeshProUGUI Tmpro_ui { get; }
    private TextMeshPro Tmpro_world { get; }
    public TMP_Text Tmpro => Tmpro_ui != null ? Tmpro_ui : Tmpro_world;
    private Coroutine Co_BuildingProcess { get; set; }

    public bool IsBuilding => Co_BuildingProcess != null;
    //public int preTextLength = 0;
    public int CharactersPerCycle { get { return (int)CurrentTextSpeed; } }

    private float TextSpeed => DialogueSystem.Instance.TextSpeed;
    [SerializeField]
    private float _TextSpeedMultiplier = 1f;
    public float CurrentTextSpeed { get { return TextSpeed * _TextSpeedMultiplier; } set { _TextSpeedMultiplier = value; } }

    public string CurrentText => Tmpro.text;
    public string TargetText { get; private set; } = "";
    public string PreText { get; private set; } = "";

    public string FullTargetText => TargetText + PreText;

    public enum TextBuildMethod { instant, typewriter }
    [SerializeField]
    private TextBuildMethod _BuildMethod = TextBuildMethod.typewriter;
    public TextBuildMethod BuildMethod => _BuildMethod;
    public Color TextColor { get { return Tmpro.color; } set { Tmpro.color = value; } }

    /*依据点击次数不同加快文本速度（预留位）*/

    #endregion
    #region 方法/Method
    //构造函数(两种)
    public TextArchitect(TextMeshProUGUI tmpro_ui)
    {
        Tmpro_ui = tmpro_ui;
    }
    public TextArchitect(TextMeshPro tmpro_world)
    {
        Tmpro_world = tmpro_world;
    }
    public Coroutine Build(string text)
    {
        PreText = "";
        if (DialogueSystem.Instance.ConversationManager.IsCharacterSpeaking)
        {
            TargetText = $"\"{text}\"";
        }
        else
        {
            TargetText = text;
        }
        //关闭现有进程
        Stop();
        //开始构建进程
        Co_BuildingProcess = Tmpro.StartCoroutine(Building());
        return Co_BuildingProcess;
    }
    public void Stop()
    {
        if (!IsBuilding)
            return;
        Tmpro.StopCoroutine(Co_BuildingProcess);
        Co_BuildingProcess = null;
    }
    public Coroutine Append(string text)
    {
        if (DialogueSystem.Instance.ConversationManager.IsCharacterSpeaking)
        {
            PreText = Tmpro.text.TrimEnd('\"');
            TargetText = $"{text}\"";
        }
        else
        {
            PreText = Tmpro.text;
            TargetText = text;
        }
        //关闭现有进程
        Stop();
        //开始构建进程
        Co_BuildingProcess = Tmpro.StartCoroutine(Building());
        return Co_BuildingProcess;
    }
    public void SetText(string text)
    {
        PreText = "";
        TargetText = text;
        Stop();
        Tmpro.text = TargetText;
        ForceComplete();
    }
    public void ForceComplete()
    {
        Tmpro.ForceMeshUpdate();
        switch (BuildMethod)
        {
            case TextBuildMethod.typewriter:
                Tmpro.maxVisibleCharacters = Tmpro.textInfo.characterCount;
                break;
        }
        Stop();
        OnComplete();
    }
    private void OnComplete()
    {
        Co_BuildingProcess = null;
    }
    private IEnumerator Building()
    {
        //开始前准备
        Prepare();
        switch (BuildMethod)
        {
            case TextBuildMethod.typewriter:
                yield return Build_Typewriter();
                break;
                //case TextBuildMethod.fade:
        }
        OnComplete();
    }
    private void Prepare()
    {
        switch (BuildMethod)
        {
            case TextBuildMethod.typewriter:
                Preapre_Typewriter();
                break;
            case TextBuildMethod.instant:
                Preapre_Instant();
                break;
        }
    }
    private void Preapre_Instant()
    {
        //颜色重载
        Tmpro.color = Tmpro.color;
        Tmpro.text = FullTargetText;
        //更新
        Tmpro.ForceMeshUpdate();
        //设置最大可见文本长度
        Tmpro.maxVisibleCharacters = Tmpro.textInfo.characterCount;
    }
    private void Preapre_Typewriter()
    {
        //颜色重载
        Tmpro.color = Tmpro.color;
        Tmpro.text = FullTargetText;
        //初始不显示文本
        Tmpro.maxVisibleCharacters = 0;
        Tmpro.text = PreText;
        //预文本非空
        if (PreText != "")
        {
            Tmpro.ForceMeshUpdate();
            Tmpro.maxVisibleCharacters = Tmpro.textInfo.characterCount;
        }
        Tmpro.text += TargetText;
        Tmpro.ForceMeshUpdate();
    }
    private IEnumerator Build_Typewriter()
    {
        while (Tmpro.maxVisibleCharacters < Tmpro.textInfo.characterCount)
        {
            Tmpro.maxVisibleCharacters += CharactersPerCycle;
            yield return new WaitForSeconds(0.015f / CurrentTextSpeed);
        }
    }

    #endregion
}
