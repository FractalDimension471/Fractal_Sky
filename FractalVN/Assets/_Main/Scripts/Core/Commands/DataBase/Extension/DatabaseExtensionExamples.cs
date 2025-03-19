using COMMANDS;
using System;
using System.Collections;
using UnityEngine;

namespace TESTING
{
    /// <summary>
    /// 指令数据扩展集
    /// </summary>
    public class DataExtinctionExamples : DatabaseExtention
    {
        new public static void Extend(CommandDatabase database)
        {
            //使用不含参数方法添加指令，每个指令对应一个实现方法（行为）
            database.AddCommand("print", new Action(PrintDefaultMessage));
            database.AddCommand("print_sp", new Action<string>(PrintLine));
            database.AddCommand("print_mp", new Action<string[]>(PrintLine));
            //通过lambada表达式实现匿名方法的定义
            database.AddCommand("lambada", new Action(() => { Debug.Log("This is a lambada expression test."); }));
            database.AddCommand("lambada_sp", new Action<string>((arg) => { Debug.Log($"User message in lambada:'{arg}'"); }));
            database.AddCommand("lambada_mp", new Action<string[]>((args) => { Debug.Log(string.Join(' ', args)); }));//连接，以空格组成的多行信息
                                                                                                                      //通过协程实现具体方法（播放音乐等操作的实现）
            database.AddCommand("process", new Func<IEnumerator>(TestProcess));
            database.AddCommand("process_sp", new Func<string, IEnumerator>(LineProcess));
            database.AddCommand("process_mp", new Func<string[], IEnumerator>(LineProcess));
            database.AddCommand("moveCharDemo", new Func<string, IEnumerator>(MoveCharacter));
        }
        private static void PrintDefaultMessage()
        {
            Debug.Log("This is the default message.");
        }
        private static void PrintLine(string line)
        {
            Debug.Log($"User message:'{line}'");
        }
        //重载方法
        private static void PrintLine(string[] lines)
        {
            int t = 0;
            foreach (string line in lines)
            {
                Debug.Log($"{t++}. {line}");
            }
        }

        private static IEnumerator TestProcess()
        {
            for (int t = 0; t < 5; t++)
            {
                Debug.Log($"Test process [{t + 1}]");
                yield return new WaitForSeconds(1);
            }
        }
        private static IEnumerator LineProcess(string line)
        {
            if (int.TryParse(line, out int num))
            {
                for (int t = 0; t < num; t++)
                {
                    Debug.Log($"Line process [{t + 1}]");
                    yield return new WaitForSeconds(1);
                }
            }
        }
        //重载方法
        private static IEnumerator LineProcess(string[] lines)
        {
            foreach (string line in lines)
            {
                Debug.Log($"Line message:'{line}'");
                yield return new WaitForSeconds(0.5f);
            }
        }

        private static IEnumerator MoveCharacter(string direction)
        {
            bool left = direction.ToLower() == "left";
            Transform character = GameObject.Find("Image").transform;
            float moveSpeed = 12;
            //当前位置
            float currentX = character.position.x;
            //目标位置（判断在左边或者右边）
            float targetX = left ? -8 : 8;

            while (Mathf.Abs(targetX - currentX) > 0.1f)
            {
                //Debug.Log($"Move character to {(left ? "left" : "right")}[{currentX}/{targetX}]");
                currentX = Mathf.MoveTowards(currentX, targetX, moveSpeed * Time.deltaTime);
                character.position = new Vector2(currentX, character.position.y);
                yield return null;
            }
        }
    }
}