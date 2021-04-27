using System;
using System.Reflection;
using System.Collections.Generic;
using ColossalFramework;
using UnityEngine;
using MoveIt;
using HarmonyLib;


namespace Propitize
{
    public enum ToolAction
    {
        None,
        Do,
        Undo,
        Redo
    }
 
    public class PropitizeAction : MoveIt.Action
    {
        public HashSet<InstanceState> m_states = new HashSet<InstanceState>();
        public bool replaceInstances = true;
        internal HashSet<Instance> m_clones; // the resulting Instances
        internal HashSet<Instance> m_oldSelection; // The selection before cloning

        public PropitizeAction(bool defaultConstructor) : base()
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


        private void StartConversion()
        {
            m_clones = new HashSet<Instance>();

            foreach (InstanceState state in m_states)
            {
                if (state is TreeState)
                {
                    lock(state.instance.data)
                    {
                        TreeInfo treeInfo = state.instance.Info.Prefab as TreeInfo;
                        PropInfo propInfo = null;
                        Instance instanceProp = null;
                        if (!PropitizeMod.PropitizedTreeMap.ContainsKey(treeInfo))
                        {
                            propInfo = PropitizeTool.PropitizeTree(treeInfo, true);
                            PropitizeTool.PropFinalTouch(ref propInfo, ref treeInfo);
                            PropitizeTool.InitializeAndBindPrefab(propInfo);
                        }
                        else
                        {
                            propInfo = PropitizeMod.PropitizedTreeMap.GetValueSafe<TreeInfo, PropInfo>(treeInfo);
                        }
                        if (Singleton<PropManager>.instance.CreateProp(out ushort clone, ref SimulationManager.instance.m_randomizer, propInfo, state.position, state.angle, true))
                        {
                            InstanceID cloneID = default;
                            cloneID.Prop = clone;
                            Debug.Log($"Propitize: Created Prop with id = {cloneID.Prop}");
                            instanceProp = new MoveableProp(cloneID);
                        }
                        m_clones.Add(instanceProp);
                        selection.Add(instanceProp);
                        selection.Remove(state.instance);
                        state.instance.Delete();
                        // save tree name to file for later propitizing
                        PropitizeTool.SavePropitizedTree(treeInfo);
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