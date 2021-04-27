using ICities;
using System.Collections.Generic;
using CitiesHarmony.API;



namespace Propitize
{
    public class PropitizeMod : IUserMod
    {
        public string Name => "Propitize 0.1";
        public string Description => "Converts Trees to Props. Integrated into Move it";
        public static bool PrefabsInitialized = false;
        public static PropInfo templateProp;
        public static Dictionary<TreeInfo, PropInfo> PropitizedTreeMap = new Dictionary<TreeInfo, PropInfo>();

        public void OnEnabled()
        {
            HarmonyHelper.DoOnHarmonyReady(() => PropitizePatch.PatchAll());
            XMLUtils.LoadSettings();
        }

        public void OnDisabled()
        {
            if (HarmonyHelper.IsHarmonyInstalled) PropitizePatch.UnpatchAll();
        }
/*
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
*/
    }
}