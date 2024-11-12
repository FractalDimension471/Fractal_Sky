using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;

public class testPanels : MonoBehaviour
{
    private InputPanel inputPanel = null;
    private ChoicePanel ChoicePanel => ChoicePanel.Instance;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TestChoicePanel());
    }
    private IEnumerator TestChoicePanel()
    {
        string[] choices = new string[]
        {
            "1","2","3"
        };
        ChoicePanel.Show("1+1=?", choices);

        //等待点击事件
        while (ChoicePanel.IsWaitingOnUserMakingChoice)
        {
            yield return null;
        }
        var decision = ChoicePanel.LastDecision;
        Debug.Log($"做出了选择{decision.answerIndex}，选择了{decision.choices[decision.answerIndex]}");
    }
    private IEnumerator TestInputPanel()
    {
        GraphicPanel panel = GraphicPanelManager.Instance.GetGraphicPanel("BackGround");
        GraphicLayer layer0 = panel.GetGraphicLayer(0, true);
        layer0.SetTexture(FilePaths.GetPath(FilePaths.DefaultImagePaths, "04"));
        Character Lacan = CharacterManager.Instance.CreateCharacter("Lacan", true);
        yield return Lacan.Say("你好，来访者。");
        yield return Lacan.Say("告诉我你的名字吧");
        inputPanel.Show("自称是？");
        while (inputPanel.IsWaitingOnUserInput)
        {
            yield return null;
        }
        string name = inputPanel.LastInput;
        yield return Lacan.Say($"好的{name}，让我们开始这次的分析。");
        yield return Lacan.Say("不过，先给我五十法郎作为定金，谢谢 :D");

    }

}
