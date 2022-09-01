using System;
using HarmonyLib;

namespace NomaiVROnlinePatches
{
    public class MessagePatches
    {
        private const string k_MessageHandlerQualifiedTypeName = "OuterWildsOnline.MessageHandler, OuterWildsMMO";

        //disables ability to equip recording tool
        [HarmonyPatch(k_MessageHandlerQualifiedTypeName, "StartPlacing")]
        [HarmonyPrefix]
        public static bool RecordingTool_StartPlacing(Object __instance)
        {
            return false;
        }
        
        //disables button prompt
        [HarmonyPatch(k_MessageHandlerQualifiedTypeName, "OnSuitUp")]
        [HarmonyPrefix]
        public static bool RecordingTool_OnSuitUp(Object __instance)
        {
            return false;
        }
    }
}