using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
public class testAudio : MonoBehaviour
{
    private Character CreatCharacter(string name) => CharacterManager.Instance.CreateCharacter(name);
    // Start is called before the first frame update
    void Start()
    {
        
        StartCoroutine(TestGA());
    }
    IEnumerator TestA()
    {
        AudioManager.Instance.PlayMusic("Audio/Music/Summer Sky");
        yield return new WaitForSeconds(5);
        AudioManager.Instance.PlayMusic("Audio/Music/Moon Light");
    }
    IEnumerator TestGA()
    {
        AudioChannel channel = new AudioChannel(1);

        CharacterSprite Chisa = CreatCharacter("Chisa") as CharacterSprite;
        Character Narrator = CreatCharacter("Narrator");
        Chisa.Show();
        yield return Narrator.Say("���");
        AudioManager.Instance.PlayVoice("Audio/Voice/Windows StartUp");
        Chisa.Animate("Hop");
        yield return Chisa.Say("����ˤ��ϡ�������ɥ��������Ӥ����衫");
        GraphicPanel BGpanel = GraphicPanelManager.Instance.GetGraphicPanel("BackGround");
        BGpanel.GetGraphicLayer(1, true).SetTexture("Graphics/BG Images/Spaceshipinterior");
        BGpanel.GetGraphicLayer(0, true).SetVideo("Graphics/BG Videos/Nebula");
        Chisa.Animate("Hop");
        yield return Chisa.Say("���á�����Ϥɤ���");
        yield return Narrator.Say("It seems that we are in a spaceship.");
        Chisa.Animate("Hop");
        yield return Chisa.Say("�����ϡ�ӳ���ߤ����Ǥ��礦��");
        yield return Narrator.Say(@"Actually, this place reminds me about the movie Stella Traverse.");
        Chisa.Animate("Hop");
        yield return Chisa.Say("������������ӳ�����ä���");
        yield return Narrator.Say("Me too. By the way, I want to sing a lyrics called Do not go gental into that good night.");
        yield return Narrator.Say("Did you want to hear it?");
        Chisa.Animate("Hop");
        yield return Chisa.Say("������");
        AudioManager.Instance.PlayMusic("Audio/Music/S.T.A.Y", loop: false, startVolume: 1);
        List<string> d01 = new List<string>()
        {
            "Do not go gentle into that good night",
            "Old age should burn and rave at close of day",
            "Rage, rage against the dying of the light",
            "Though wise men at their end know dark is right",
            "Because their words had forked no lightning they",
            "Do not go gentle into that good night",
            "Good men",
            "the last wave by",
            "crying how brightTheir frail deeds might have danced in a green bay",
            "Rage, rage against the dying of the light",
            "Wild men",
            "Who caught and sang the sun in flight",
            "And learn",
            "Too late",
            "They grieved it on its way",
            "Do not go gentle into that good night",
            "Grave men",
            "Near death",
            "Who see with blinding sightBlind eyes" +
            "Could blaze like meteors and be gay",
            "Rage, rage against the dying of the light",
            "And you",
            "My father",
            "There on the sad height",
            "Curse, bless me now with your fierce tears, I pray",
            "Do not go gentle into that good night",
            "Rage, rage against the dying of the light"
        };
        yield return Narrator.Say(d01);


        
        
    }

}
