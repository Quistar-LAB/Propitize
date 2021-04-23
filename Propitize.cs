using System;
using System.Collections.Generic;
using ICities;
using UnityEngine;
using CitiesHarmony.API;



namespace Propitize
{
    public class Mod : IUserMod
    {
        public string Name => "Propitize 0.1";
        public string Description => "Converts Trees to Props. Integrated into Move it";

        public static Dictionary<PropInfo, PropInfo> convertedTreePropMap = new Dictionary<PropInfo, PropInfo>();

        public void OnEnabled()
        {
            HarmonyHelper.DoOnHarmonyReady(() => PropitizePatch.PatchAll());
        }

        public void OnDisabled()
        {
            if (HarmonyHelper.IsHarmonyInstalled) PropitizePatch.UnpatchAll();
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            try
            {

            }
            catch (Exception e)
            {
                Debug.Log("OnSettingsUI failed");
                Debug.LogException(e);
            }
        }
    }
}