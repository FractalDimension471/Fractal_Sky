using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;
namespace TESTING
{
    public class testParser : MonoBehaviour
    {
        [SerializeField] private TextAsset textFile;
        // Start is called before the first frame update
        void Start()
        {
            //string line = "speakerA@\"Dialogue gose here.\"@command(code:01)";
            //DialogueParser.parse(line);
            SendFileToParse();
        }
        void SendFileToParse()
        {
            List<string> lines = FileManager.ReadTextAsset(textFile,false);
            foreach(string line in lines)
            {
                DialogueLine dl = DialogueParser.Parse(line);
            }
        }
    }
}

