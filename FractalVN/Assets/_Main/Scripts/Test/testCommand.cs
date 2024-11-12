using COMMANDS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class testCommand : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Running());

        }
        private void Update()
        {
            /*
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                CommandManager.instance.Excute("moveCharDemo", "left");
            }
            else if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                CommandManager.instance.Excute("moveCharDemo", "right");
            }
            */
        }
        IEnumerator Running()
        {
            yield return CommandManager.Instance.Execute("print");
            yield return CommandManager.Instance.Execute("print_sp", "Welcome!");
            yield return CommandManager.Instance.Execute("print_mp", "Welcome!", "Nice to meet you.", "How are you?");

            yield return CommandManager.Instance.Execute("lambada");
            yield return CommandManager.Instance.Execute("lambada_sp", "Welcome!");
            yield return CommandManager.Instance.Execute("lambada_mp", "Welcome!", "Nice to meet you.", "How are you?");

            yield return CommandManager.Instance.Execute("process");
            yield return CommandManager.Instance.Execute("process_sp", "5");
            yield return CommandManager.Instance.Execute("process_mp", "Welcome!", "Nice to meet you.", "How are you?");
        }
    }
}