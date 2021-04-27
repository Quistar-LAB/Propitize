// Most of the codes here referenced from Elektrix's Tree and Vehicle Props Mod
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoveIt;

namespace Propitize
{
    public partial class PropitizeTool : ToolBase
    {
        public static PropitizeTool instance;
        public ToolAction m_nextAction = ToolAction.None;

        public void StartConvertAction()
        {
            if (ActionQueue.instance == null)
            {
                return;
            }

            if (MoveIt.Action.selection.Count > 0)
            {
                PropitizeAction action = new PropitizeAction(true);
                lock (ActionQueue.instance)
                {
                    ActionQueue.instance.Push(action);
                }
            }
        }
        // Acquiring and parsing move it selections
        static public List<ushort> ExtractPropsFromMoveItSelection()
        {
            HashSet<Instance> MoveItSelection = MoveIt.Action.selection;

            List<ushort> formattedProps = new List<ushort>();

            foreach (Instance objectInstance in MoveItSelection)
            {
                InstanceID unreflected = objectInstance.id;
                if (unreflected.Type == InstanceType.Prop)
                {
                    formattedProps.Add(unreflected.Prop);
                }
            }

            return formattedProps;
        }

        public static IEnumerator GeneratePropitizedTreeFromFile()
        {
            foreach(PropitizedTreeEntry entry in Settings.PropitizedTreeEntries)
            {
                TreeInfo treeInfo = PrefabCollection<TreeInfo>.FindLoaded(entry.name);
                PropInfo propInfo = PropitizeTree(treeInfo, false);
                PropitizeMod.PropitizedTreeMap.Add(treeInfo, propInfo);
                yield return null;
            }
        }
        
        public static IEnumerator InitializeAndBindPrefab()
        {
            PrefabCollection<PropInfo>.InitializePrefabs("Propitized Tree", PropitizeMod.PropitizedTreeMap.Select((KeyValuePair<TreeInfo, PropInfo> k) => k.Value).ToArray(), null);
            yield return null;
            PrefabCollection<PropInfo>.BindPrefabs();
            yield return null;
        }
        
        public static IEnumerator InitializeAndBindPrefab(PropInfo propInfo)
        {
            PrefabCollection<PropInfo>.InitializePrefabs("Propitized Tree", propInfo, null);
            PrefabCollection<PropInfo>.BindPrefabs();
            return null;
        }

        public static void SavePropitizedTree(TreeInfo tree)
        {
            try
            {
                PropitizedTreeEntry entry = new PropitizedTreeEntry(tree.name);
                if (Settings.PropitizedTreeEntries.Exists(x => x.name.Contains(tree.name))) return;
                Settings.PropitizedTreeEntries.Add(entry);
                XMLUtils.SaveSettings();
            }
            catch (Exception e)
            {
                Debug.Log("Propitize: Exception occurred while saving data");
                Debug.LogException(e);
            }
        }

        public static PropInfo CreateClone(bool active)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(PropitizeMod.templateProp.gameObject);
            gameObject.SetActive(active);
            gameObject.GetComponent<PrefabInfo>().m_isCustomContent = true;
            return gameObject.GetComponent<PropInfo>();
        }

        public static PropInfo PropitizeTree(TreeInfo treeInfo, bool active)
        {
            PropInfo propInfo = CreateClone(active);

            ((UnityEngine.Object)propInfo).name = ((UnityEngine.Object)treeInfo).name.Replace("_Data", "") + "Prop_Data";

            propInfo.m_mesh = treeInfo.m_mesh;
            propInfo.m_material = treeInfo.m_material;
            propInfo.m_Thumbnail = treeInfo.m_Thumbnail;
            propInfo.m_InfoTooltipThumbnail = treeInfo.m_InfoTooltipThumbnail;
            propInfo.m_InfoTooltipAtlas = treeInfo.m_InfoTooltipAtlas;
            propInfo.m_Atlas = treeInfo.m_Atlas;
            propInfo.m_generatedInfo.m_center = treeInfo.m_generatedInfo.m_center;
            propInfo.m_generatedInfo.m_uvmapArea = treeInfo.m_generatedInfo.m_uvmapArea;
            propInfo.m_generatedInfo.m_size = treeInfo.m_generatedInfo.m_size;
            propInfo.m_generatedInfo.m_triangleArea = treeInfo.m_generatedInfo.m_triangleArea;
            propInfo.m_color0 = treeInfo.m_defaultColor;
            propInfo.m_color1 = treeInfo.m_defaultColor;
            propInfo.m_color2 = treeInfo.m_defaultColor;
            propInfo.m_color3 = treeInfo.m_defaultColor;

            return propInfo;
        }

        public static void PropFinalTouch(ref PropInfo propInfo, ref TreeInfo tree)
        {

            propInfo.m_lodMesh = tree.m_mesh;
            propInfo.m_lodMaterial = tree.m_material;
            propInfo.m_lodObject = propInfo.gameObject;
            propInfo.m_generatedInfo = UnityEngine.Object.Instantiate<PropInfoGen>(propInfo.m_generatedInfo);
            propInfo.m_generatedInfo.name = propInfo.name;
            propInfo.m_isCustomContent = tree.m_isCustomContent;
            propInfo.m_dlcRequired = tree.m_dlcRequired;
            propInfo.m_generatedInfo.m_propInfo = propInfo;
            if (propInfo.m_mesh != null)
            {
                Vector3 one = Vector3.one;
                Bounds bounds1 = propInfo.m_mesh.bounds;
                var x = bounds1.extents.x;
                Bounds bounds2 = propInfo.m_mesh.bounds;
                var y = bounds2.extents.y;
                Bounds bounds3 = propInfo.m_mesh.bounds;
                var z = bounds3.extents.z;
                double num1 = (double)Math.Max((float)y, (float)z);
                double num2 = (double)Math.Max((float)x, (float)num1) * 2.0 - 1.0;
                Vector3 vector3 = one * (float)num2;
                propInfo.m_generatedInfo.m_size = vector3;
            }
            if (propInfo.m_material != null)
            {
                propInfo.m_material.SetColor("_ColorV0", propInfo.m_color0);
                propInfo.m_material.SetColor("_ColorV1", propInfo.m_color1);
                propInfo.m_material.SetColor("_ColorV2", propInfo.m_color2);
                propInfo.m_material.SetColor("_ColorV3", propInfo.m_color3);
            }
            PropitizeMod.PropitizedTreeMap.Add(tree, propInfo);
        }
    }
}