using System;
using System.Reflection;
using System.Text;
using HarmonyLib;
using ModTemplate;
using Sfs2X;
using UnityEngine.UI;
using Valve.VR;

namespace NomaiVROnlinePatches
{
    public class ChatPatches
    {
        private const string k_chatHandlerQualifiedTypeName = "ModTemplate.ChatHandler, OuterWildsOnline";
        private static SmartFox sfs { get => ConnectionController.Connection; }
        private static Action UpdateOpenChat = null;
        private static Type chatHandlerType = AccessTools.TypeByName(k_chatHandlerQualifiedTypeName);
        private static FieldInfo chatHandlerInputText = AccessTools.Field(chatHandlerType, "inputFieldText");
        private static InputField hiddenInputField;
        private static Object chatHandlerInstance;

        [HarmonyPatch(k_chatHandlerQualifiedTypeName, "Start")]
        [HarmonyPostfix]
        public static void ChatHandler_Start(Object __instance)
        {
            chatHandlerInstance = __instance;
            UpdateOpenChat = AccessTools.MethodDelegate<Action>(AccessTools.DeclaredMethod(chatHandlerType, "UpdateOpenChat"), __instance);
            hiddenInputField = (new UnityEngine.GameObject("VRChatField")).AddComponent<InputField>();
        }

        public static void OnSteamVRKeyboaredClosed(VREvent_t evt)
        {
            if(hiddenInputField != null && hiddenInputField.IsActive())
            {
                var text = new StringBuilder(256);
                SteamVR.instance.overlay.GetKeyboardText(text, 256);
                var message = text.ToString();
                
                if (!String.IsNullOrEmpty(message.Trim(' ')))
                {
                    sfs.Send(new Sfs2X.Requests.PublicMessageRequest(AccessTools.Field(chatHandlerType, "chatMode").GetValue(chatHandlerInstance).ToString() + "ʣ" + message));
                }
                hiddenInputField.DeactivateInputField();
                hiddenInputField.text = "";
            }
            SteamVR_Events.System(EVREventType.VREvent_KeyboardClosed).RemoveListener(OnSteamVRKeyboaredClosed);
        }

        [HarmonyPatch(k_chatHandlerQualifiedTypeName, "Update")]
        [HarmonyPrefix]
        public static bool ChatHandler_Update(Object __instance)
        {
            if(OWInput.IsPressed(InputLibrary.toolActionSecondary))
            {
                if(OWInput.IsNewlyPressed(InputLibrary.rollMode))
                {
                    UpdateOpenChat();
                }

                if(OWInput.IsNewlyPressed(InputLibrary.interact))
                {
                    SteamVR_Events.System(EVREventType.VREvent_KeyboardClosed).Listen(OnSteamVRKeyboaredClosed);
                    hiddenInputField.ActivateInputField();
                }
            }
            return false;
        }
    }
}