using System.Reflection;
using UnityEngine;
using MoveIt;
using HarmonyLib;

namespace Propitize
{
    public static class PropitizePatch
    {
        private const string HarmonyId = "Propitize";
        private static bool patched = false;

        public static void PatchAll()
        {
            if (patched) return;

            patched = true;

            var harmony = new Harmony("Propitize");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        public static void UnpatchAll()
        {
            if (!patched) return;

            var harmony = new Harmony(HarmonyId);
            harmony.UnpatchAll(HarmonyId);

            patched = false;
        }

        public static class PropitizeInstallation
        {
            // Attach button to MoveIt mod panel
            [HarmonyPatch(typeof(UIToolOptionPanel), "Start")]
            public static void Postfix()
            {
                if (UIToolOptionPanel.instance == null) return;

                // Initilization
                ToolController toolController = Object.FindObjectOfType<ToolController>();
                PropitizeAction.instance = toolController.gameObject.AddComponent<PropitizeAction>();

                PropitizeButton.CreateSubButton(UIToolOptionPanel.instance, "Propitize", "Propitize", "Propitize");
            }

            // Get selection list and perform conversion
            [HarmonyPatch(typeof(SelectAction), "Add")]
            public static class PropitizeMoveItSelectionBinderPatch
            {
                private static void Postfix()
                {
                    PropitizeAction.ExtractPropsFromMoveItSelection();
                }
            }
        }
    }
}