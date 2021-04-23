using System.Reflection;
using System.Collections.Generic;
using ICities;
using UnityEngine;
using ColossalFramework.UI;
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

            // Apply your patches here!
            // Harmony.DEBUG = true;
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

        [HarmonyPatch(typeof(UIToolOptionPanel), "Start")]
        public static class PropitizeInstallation
        {
            public static void Postfix()
            {
                if (UIToolOptionPanel.instance == null) return;

                // Initilization
                ToolController toolController = Object.FindObjectOfType<ToolController>();
                PropitizeAction.instance = toolController.gameObject.AddComponent<PropitizeAction>();

                PropitizeButton.CreateSubButton(UIToolOptionPanel.instance, "Propitize", "Propitize", "Propitize");
            }

            [HarmonyPatch(typeof(SelectAction), "Add")]
            public static class PropitizeMoveItSelectionBinderPatch
            {
                private static void Postfix()
                {
                    List<ushort> t = PropitizeAction.ExtractPropsFromMoveItSelection();
                    for (int i = 0; i < t.Count; i++)
                    {
                       // Db.w((i + 1) + ": " + t[i]);
                    }
                }
            }
        }
    }
}