using GALGAME;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{
    public class CoreExtension : DatabaseExtention
    {
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("setplayername", new Action<string>(SetPlayerName));
        }

        private static void SetPlayerName(string data)
        {
            GalSaveFile.ActiveFile.PlayerName = data;
        }
    }
}