using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HISTORY;
using System;
public class TestHistory : MonoBehaviour
{
    public HistoryState state;
    // Start is called before the first frame update
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            state = HistoryState.Capture();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            state.Load();
        }
    }
}
