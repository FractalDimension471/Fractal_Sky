using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
using DIALOGUE;
namespace TESTING
{
    public class testCharacters : MonoBehaviour
    {
        private Character CreatCharacter(string name) => CharacterManager.Instance.CreateCharacter(name);
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(TestCharacter01());
        }
        IEnumerator TestCharacter01()
        {
            Character Aoichan = CreatCharacter("Aoi");
            CharacterSprite Kaikou = CreatCharacter("Kaikou") as CharacterSprite;
            Character N = CreatCharacter("N as Generic");
            yield return N.Say("һ��������糿");
            yield return Aoichan.Show();
            List<string> text01 = new List<string>
            {
                "��������̫������......",
                "�ܸо�����Ľ��Һð�����",
                "��ʦ������ͬѧ��û��������...",
                "...",
                "!"
            };
            yield return Aoichan.Say(text01);
            yield return Aoichan.MoveToPosition(new Vector2(1, 0));
            Kaikou.SetPosition(new Vector2(-0.5f, 0));
            yield return Kaikou.Show();
            Kaikou.SetSprite(Kaikou.GetSprite("fe02"), 1);
            yield return Kaikou.MoveToPosition(new Vector2(0, 0),speed:6);
            Aoichan.Animate("Hop");
            yield return Aoichan.Say("�����Q��ͬѧ");
            Kaikou.Animate("Shiver", true);
            yield return Kaikou.Say("�������......");
            Aoichan.Animate("Hop");
            yield return Aoichan.Say("�Ҹպ���һ����ţ�̣����͸��Q��ͬѧ��");
            yield return Kaikou.Say("��......лл");
            yield return N.Say("�Q��ܿ�ͺ������Ǳ���ţ��");
            Kaikou.Animate("Shiver", false);
            Kaikou.SetSprite(Kaikou.GetSprite("fe01"), 1);
            yield return Kaikou.Say("......��������");
            Aoichan.Animate("Hop");
            yield return Aoichan.Say("�ٺ�~���Q��ͬѧû�¾ͺ�");
        }
        IEnumerator TestCharacter02()
        {
            Character Chisa = CreatCharacter("Chisa");
            yield return Chisa.Show();
            Chisa.Say("����ˤ���");
        }
        
    }
}