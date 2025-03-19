using System;
using System.Collections;

namespace COMMANDS
{
    public class GalleryExtension : DatabaseExtention
    {
        private static string[] ID_ImageName { get; } = { "/n", "/name" };
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("unlockcg", new Func<string[], IEnumerator>(UnlockCG));
            database.AddCommand("eraseallcgs", new Action(EraseAllCGs));
        }
        private static IEnumerator UnlockCG(string[] data)
        {
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(ID_ImageName, out string cgName);
            GalleryConfig.UnlockCG(cgName);
            yield return null;
        }
        private static void EraseAllCGs() => GalleryConfig.Erase();
    }
}