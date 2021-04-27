// modified from Elektrix's Tree and Vehicle Props
using System.Collections.Generic;
using ICities;
using UnityEngine;

namespace Propitize
{
    public class LoadingExtension : LoadingExtensionBase
    {
         public override void OnLevelLoaded(LoadMode mode)
        {
            if(!PropitizeMod.PrefabsInitialized)
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
