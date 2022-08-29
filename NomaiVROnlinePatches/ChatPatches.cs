using System;
using System.Reflection;
using System.Text;
using HarmonyLib;
using OuterWildsOnline;
using Sfs2X;
using UnityEngine.UI;
using Valve.VR;

namespace NomaiVROnlinePatches
{
    public class ChatPatches
    {
        private const string k_chatHandlerQualifiedTypeName = "OuterWildsOnline.ChatHandler, OuterWildsMMO";
        private static SmartFox sfs { get => ConnectionController.Connection; }
        private static Action UpdateOpenChat = null;
        private static ScreenPrompt enterChatPrompt;
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

            enterChatPrompt = new ScreenPrompt(InputLibrary.toolActionSecondary, InputLibrary.interact, "<CMD1>+<CMD2> Start chatting", ScreenPrompt.MultiCommandType.CUSTOM_BOTH);
            Locator.GetPromptManager().RemoveScreenPrompt((ScreenPrompt)AccessTools.Field(chatHandlerType, "enterChatPrompt").GetValue(__instance));
            AccessTools.Field(chatHandlerType, "enterChatPrompt").SetValue(__instance, enterChatPrompt);
            Locator.GetPromptManager().AddScreenPrompt(enterChatPrompt, PromptPosition.UpperRight, true);
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
            /*if(OWInput.IsPressed(InputLibrary.toolActionSecondary))
            {
                if(OWInput.IsNewlyPressed(InputLibrary.rollMode) && !IsChatNA(__instance))
                {
                    UpdateOpenChat();
                }

                if(OWInput.IsNewlyPressed(InputLibrary.interact) && !IsChatNA(__instance))
                {
                    SteamVR_Events.System(EVREventType.VREvent_KeyboardClosed).Listen(OnSteamVRKeyboaredClosed);
                    hiddenInputField.ActivateInputField();
                }
            }*/
            return false;
        }

        //Until ChatHandler isn't public this is the only way to access the enum
        private static bool IsChatNA(Object chatHandler) => (int)AccessTools.Field(chatHandlerType, "chatMode").GetValue(chatHandlerInstance) == 0;
    }
}