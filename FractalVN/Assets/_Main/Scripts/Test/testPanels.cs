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

        //�ȴ�����¼�
        while (ChoicePanel.IsWaitingOnUserMakingChoice)
        {
            yield return null;
        }
        var decision = ChoicePanel.LastDecision;
        Debug.Log($"������ѡ��{decision.answerIndex}��ѡ����{decision.choices[decision.answerIndex]}");
    }
    private IEnumerator TestInputPanel()
    {
        GraphicPanel panel = GraphicPanelManager.Instance.GetGraphicPanel("BackGround");
        GraphicLayer layer0 = panel.GetGraphicLayer(0, true);
        layer0.SetTexture(FilePaths.GetPath(FilePaths.DefaultImagePaths, "04"));
        Character Lacan = CharacterManager.Instance.CreateCharacter("Lacan", true);
        yield return Lacan.Say("��ã������ߡ�");
        yield return Lacan.Say("������������ְ�");
        inputPanel.Show("�Գ��ǣ�");
        while (inputPanel.IsWaitingOnUserInput)
        {
            yield return null;
        }
        string name = inputPanel.LastInput;
        yield return Lacan.Say($"�õ�{name}�������ǿ�ʼ��εķ�����");
        yield return Lacan.Say("�������ȸ�����ʮ������Ϊ����лл :D");

    }

}
