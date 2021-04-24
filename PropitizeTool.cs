using System;
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
                PropitizeActionBase action = new PropitizeActionBase(true);
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

        internal static void Load()
        {
            try
            {

            }
            catch (Exception e)
            {
                Debug.Log("Exception occurred while reading data");
                Debug.LogException(e);
            }
        }

        internal static void Save()
        {
            try
            {

            }
            catch (Exception e)
            {
                Debug.Log("Exception occurred while saving data");
                Debug.LogException(e);
            }
        }
    }
}