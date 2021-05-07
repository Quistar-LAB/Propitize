using ICities;
using System.Collections.Generic;
using CitiesHarmony.API;
using UnityEngine;



namespace Propitize
{
    public class PropitizeMod : LoadingExtensionBase, IUserMod
    {
        public static bool PrefabsInitialized = false;
        public static PropInfo templateProp;
        public static Dictionary<TreeInfo, PropInfo> PropitizedTreeMap = new Dictionary<TreeInfo, PropInfo>();

        #region IUserMod implementation
        public static readonly string version = "0.1";

        public string Name
        {
            get { return "Propitize " + version; }
        }

        public string Description
        {
            get { return "Converts Trees to Props. Integrated into Move it"; }
        }

        public void OnEnabled()
        {
            HarmonyHelper.DoOnHarmonyReady(() => PropitizePatch.PatchAll());
            XMLUtils.LoadSettings();
        }

        public void OnDisabled()
        {
            if (HarmonyHelper.IsHarmonyInstalled) PropitizePatch.UnpatchAll();
        }
        #endregion

        #region LoadingExtensionBase overrides
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (!PropitizeMod.PrefabsInitialized)
            {
                PropitizeMod.PrefabsInitialized = true;
                foreach (KeyValuePair<TreeInfo, PropInfo> keyValuePair in PropitizeMod.PropitizedTreeMap)
                {
                    PropInfo propInfo = keyValuePair.Value;
                    TreeInfo treeInfo = keyValuePair.Key;
                    PropitizeTool.PropFinalTouch(ref propInfo, ref treeInfo);
                }
            }
        }

        public override void OnLevelUnloading()
        {
            PropitizeMod.PrefabsInitialized = false;
        }
        #endregion
    }
    public class AssetExtension : AssetDataExtensionBase
    {
        public override void OnAssetLoaded(string name, object asset, Dictionary<string, byte[]> userData)
        {
            if (!(asset is PropInfo)) return;
            PropInfo propInfo = asset as PropInfo;
            if (((Object)propInfo).name.EndsWith("ElektrixPropConversionTemplate_Data"))
            {
                PropitizeMod.templateProp = propInfo;
            }
        }
    }
}