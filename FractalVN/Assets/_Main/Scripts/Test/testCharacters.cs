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
            yield return N.Say("一个冬天的早晨");
            yield return Aoichan.Show();
            List<string> text01 = new List<string>
            {
                "今天来得太早了呢......",
                "总感觉今天的教室好安静啊",
                "老师和其他同学都没有在这里...",
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
            yield return Aoichan.Say("啊，芉香同学");
            Kaikou.Animate("Shiver", true);
            yield return Kaikou.Say("外面好冷......");
            Aoichan.Animate("Hop");
            yield return Aoichan.Say("我刚好有一杯热牛奶，就送给芉香同学吧");
            yield return Kaikou.Say("好......谢谢");
            yield return N.Say("芉香很快就喝完了那杯热牛奶");
            Kaikou.Animate("Shiver", false);
            Kaikou.SetSprite(Kaikou.GetSprite("fe01"), 1);
            yield return Kaikou.Say("......缓过来了");
            Aoichan.Animate("Hop");
            yield return Aoichan.Say("嘿嘿~，芉香同学没事就好");
        }
        IEnumerator TestCharacter02()
        {
            Character Chisa = CreatCharacter("Chisa");
            yield return Chisa.Show();
            Chisa.Say("こんにちは");
        }
        
    }
}