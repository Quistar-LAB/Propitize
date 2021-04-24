using System;
using System.Collections.Generic;
using ColossalFramework;
using UnityEngine;
using MoveIt;


namespace Propitize
{
    public enum ToolAction
    {
        None,
        Do,
        Undo,
        Redo
    }
 
    public class PropitizeActionBase : MoveIt.Action
    {
        public HashSet<InstanceState> m_states = new HashSet<InstanceState>();
        public bool replaceInstances = true;
        internal HashSet<Instance> m_clones; // the resulting Instances
        internal HashSet<Instance> m_oldSelection; // The selection before cloning

        public PropitizeActionBase(bool defaultConstructor) : base()
        {
            if (!defaultConstructor) return; // Ugly hack to control when this constructor is called by derived classes

            m_oldSelection = selection;

            HashSet<Instance> newSelection = new HashSet<Instance>(selection);

            foreach(Instance instance in newSelection)
            {
                if(instance.id.Type == InstanceType.Tree && instance.isValid)
                {
                    m_states.Add(instance.SaveToState());
                }
            }
            StartConversion();
        }

        private static PropInfo CreateClone()
        {
            PropInfo tempProp = PrefabCollection<PropInfo>.GetLoaded(0);
            GameObject gameObject = UnityEngine.Object.Instantiate(tempProp.gameObject);
            gameObject.SetActive(value: true);
            PrefabInfo component = gameObject.GetComponent<PrefabInfo>();
            component.m_isCustomContent = true;
            return gameObject.GetComponent<PropInfo>();
        }


        private PropInfo ConvertToProp(PropInfo propInfo, Instance instance)
        {
            if (instance == null) return null;

            TreeInfo tree = instance.Info.Prefab as TreeInfo;
            if (tree == null) return null;

            propInfo.name = "Propitized_" + tree.name;
            propInfo.m_mesh = tree.m_mesh;
            propInfo.m_material = tree.m_material;
            propInfo.m_Thumbnail = tree.m_Thumbnail;
            propInfo.m_InfoTooltipThumbnail = tree.m_InfoTooltipThumbnail;
            propInfo.m_InfoTooltipAtlas = tree.m_InfoTooltipAtlas;
            propInfo.m_Atlas = tree.m_Atlas;
            propInfo.m_generatedInfo.m_center = tree.m_generatedInfo.m_center;
            propInfo.m_generatedInfo.m_uvmapArea = tree.m_generatedInfo.m_uvmapArea;
            propInfo.m_generatedInfo.m_size = tree.m_generatedInfo.m_size;
            propInfo.m_generatedInfo.m_triangleArea = tree.m_generatedInfo.m_triangleArea;
            propInfo.m_color0 = tree.m_defaultColor;
            propInfo.m_color1 = tree.m_defaultColor;
            propInfo.m_color2 = tree.m_defaultColor;
            propInfo.m_color3 = tree.m_defaultColor;

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
                if (propInfo.m_isCustomContent)
                {
                    propInfo.m_mesh = UnityEngine.Object.Instantiate<Mesh>(tree.m_mesh);
                }
                propInfo.m_generatedInfo.m_size = Vector3.one * (Math.Max(propInfo.m_mesh.bounds.extents.x, Math.Max(propInfo.m_mesh.bounds.extents.y, propInfo.m_mesh.bounds.extents.z)) * 2f - 1f);
            }
            if (propInfo.m_material != null)
            {
                propInfo.m_material.SetColor("_ColorV0", propInfo.m_color0);
                propInfo.m_material.SetColor("_ColorV1", propInfo.m_color1);
                propInfo.m_material.SetColor("_ColorV2", propInfo.m_color2);
                propInfo.m_material.SetColor("_ColorV3", propInfo.m_color3);
            }

            if (propInfo == null)
            {
                Debug.Log("Propitize: propInfo is null");
                return null;
            }
            
            return propInfo;
        }

        private void StartConversion()
        {
            m_clones = new HashSet<Instance>();

            foreach (InstanceState state in m_states)
            {
                if (state is TreeState)
                {
                    lock(state.instance.data)
                    {
                        PropInfo propInfo = CreateClone();
                        if (propInfo == null)
                        {
                            Log.Debug($"Propitize: Failed to create clone {state}");
                            continue;
                        }
                        Instance instanceProp = null;
                        if (Singleton<PropManager>.instance.CreateProp(out ushort clone, ref SimulationManager.instance.m_randomizer, propInfo, state.position, state.angle, true))
                        {
                            InstanceID cloneID = default;
                            cloneID.Prop = clone;
                            instanceProp = new MoveableProp(cloneID);
                        }
                        ConvertToProp(instanceProp.Info.Prefab as PropInfo, state.instance);
                        m_clones.Add(instanceProp);
                        selection.Add(instanceProp);
                        selection.Remove(state.instance);
                        state.instance.Delete();
                    }
                }
            }
        }

        public override void Do()
        {
            throw new NotImplementedException();
        }
        public override void Undo()
        {
            throw new NotImplementedException();
        }
        public override void ReplaceInstances(Dictionary<Instance, Instance> toReplace)
        {
            throw new NotImplementedException();
        }
    }
}