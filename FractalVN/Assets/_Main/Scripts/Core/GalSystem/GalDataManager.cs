using DIALOGUE.LogicalLine;
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