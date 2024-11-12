using DIALOGUE.LogicalLine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GALGAME
{
    public class GalDataManager : MonoBehaviour
    {
        public void SetupExternalLinks()
        {
            VariableStore.TryCreateVariable("Aurora.MainCharacterName", "", () => GalSaveFile.ActiveFile.PlayerName, value => GalSaveFile.ActiveFile.PlayerName = value);
            
        }
    }
}