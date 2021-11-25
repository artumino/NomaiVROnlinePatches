using OWML.ModHelper;
using HarmonyLib;
using System.Collections.Generic;

namespace NomaiVROnlinePatches
{
    public class NomaiVROnlinePatches : ModBehaviour
    {
        private static readonly Dictionary<string, string> VersionCheck
            = new Dictionary<string, string>
        {
            { "Raicuparta.NomaiVR", "2.4.2" },
            { "Vesper.OuterWildsMMO", "0.3.3" }
        };

        private void Start()
        {
            if(!CheckVersion())
            {
                UnityEngine.Debug.LogError("[NomaiVROnlinePatches] Version check failed, disabling mod");
                return;
            }

            Harmony.CreateAndPatchAll(typeof(ChatPatches));
        }

        private bool CheckVersion()
        {
            var versionCheck = true;
            foreach(var dependency in GetDependencies())
            {
                var manifest = dependency.ModHelper.Manifest;
                if(VersionCheck[manifest.UniqueName] != manifest.Version)
                {
                    versionCheck = false;
                    UnityEngine.Debug.LogError($"[NomaiVROnlinePatches] {manifest.UniqueName} expected {VersionCheck[manifest.UniqueName]} got {manifest.Version}");
                }
            }
            return versionCheck;
        }
    }
}
