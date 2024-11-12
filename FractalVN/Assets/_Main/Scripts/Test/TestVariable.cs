using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE.LogicalLine;

public class TestVariable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        VariableStore.TryCreateDatabase("DB01");
        VariableStore.TryCreateDatabase("DB04");
        VariableStore.TryCreateDatabase("DB07");

        VariableStore.TryCreateVariable("DB01.intV", 1);
        VariableStore.TryCreateVariable("DB01.floatV", 3.14f);
        VariableStore.TryCreateVariable("DB01.doubleV", 5.7);
        VariableStore.TryCreateVariable("DB04.boolV", true);
        VariableStore.TryCreateVariable("DB07.charV", '%');
        VariableStore.TryCreateVariable("DB07.stringV", "Hello world.");

        VariableStore.TryCreateVariable("SolidateVariable", 471);

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            VariableStore.PrintAllDatabases();
            VariableStore.PrintAllVariables();
        }
    }
}
