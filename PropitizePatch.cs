using System.Reflection;
using UnityEngine;
using MoveIt;
using HarmonyLib;
using ColossalFramework;

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

            var harmony = new Harmony(HarmonyId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        public static void UnpatchAll()
        {
            if (!patched) return;

            var harmony = new Harmony(HarmonyId);
            harmony.UnpatchAll(HarmonyId);

            patched = false;
        }
    }

    [HarmonyPatch(typeof(UIToolOptionPanel), "Start")]
    public static class PropitizeInstallation
    {
        // Attach button to MoveIt mod panel
        public static void Postfix()
        {
            if (UIToolOptionPanel.instance == null) return;

            // Initilization
            ToolController toolController = Object.FindObjectOfType<ToolController>();
            PropitizeTool.instance = toolController.gameObject.AddComponent<PropitizeTool>();

            PropitizeButton.CreateSubButton(UIToolOptionPanel.instance, "Propitize", "Propitize", "Propitize");
        }
    }

    // Get selection list and perform conversion
    [HarmonyPatch(typeof(SelectAction), "Add")]
    public static class PropitizeMoveItSelectionBinderPatch
    {
        private static void Postfix()
        {
            PropitizeTool.ExtractPropsFromMoveItSelection();
        }
    }

    [HarmonyPatch(typeof(RenderManager), "Managers_CheckReferences")]
    public static class LoadingHook
    {
        public static void Prefix()
        {
            if (!PropitizeMod.PrefabsInitialized)
            {
                PropitizeMod.PrefabsInitialized = true;
                Singleton<LoadingManager>.instance.QueueLoadingAction(PropitizeTool.GeneratePropitizedTreeFromFile());
                Singleton<LoadingManager>.instance.QueueLoadingAction(PropitizeTool.InitializeAndBindPrefab());
            }
        }
    }
}