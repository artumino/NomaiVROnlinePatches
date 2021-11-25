using OWML.ModHelper;
using HarmonyLib;

namespace NomaiVROnlinePatches
{
    public class NomaiVROnlinePatches : ModBehaviour
    {
        private void Start()
        {
            Harmony.CreateAndPatchAll(typeof(ChatPatches));
        }
    }
}
