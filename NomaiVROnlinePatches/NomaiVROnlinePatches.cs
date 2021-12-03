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
            { "Raicuparta.NomaiVR", "2.5" },
            { "Vesper.OuterWildsMMO", "0.3.5" }
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
                var installedVersion = manifest.Version;
                var requiredVersion = VersionCheck[manifest.UniqueName];
                if(!installedVersion.StartsWith(requiredVersion))
                {
                    versionCheck = false;
                    UnityEngine.Debug.LogError($"[NomaiVROnlinePatches] {manifest.UniqueName} expected {requiredVersion} got {installedVersion}");
                }
            }
            return versionCheck;
        }
    }
}
