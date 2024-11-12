using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GALGAME;
using DIALOGUE.LogicalLine;

public class TestFileSL : MonoBehaviour
{
    [SerializeField] private GalSaveFile saveFile;
    // Start is called before the first frame update
    void Start()
    {
        GalSaveFile.ActiveFile = new();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            GalSaveFile.ActiveFile.Save();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            string path = @"H:\databaseIII\LetsGalgame01\Assets\AppData\SaveData\1.fsd";
            saveFile = GalSaveFile.Load(path, activateOnLoad: true);   
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            VariableStore.PrintAllDatabases();
            VariableStore.PrintAllVariables();
        }
    }
}
