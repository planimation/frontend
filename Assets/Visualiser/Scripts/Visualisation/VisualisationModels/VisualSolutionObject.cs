

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/*
 * 
 * Purpose: The file is the solution object which has data that is sent by the server to Unity application
 * Authors: Tom, Collin, Hugo and Sharukh
 * Date: 14/08/2018
 * Reviewers: Sharukh, Gang and May
 * Review date: 10/09/2018
 * 
 * /
 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  */


using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Visualiser
{
    /* This class contains all the information in the Visualisation file, including all stages
	 * and image*/
    [Serializable]
    public class VisualSolutionObject
    {
        public VisualStageObject[] visualStages;
        public int transferType;
        public ImageDictionary imageTable;
        public Dictionary<int, string[]> subgoalMap = new Dictionary<int, string[]>();
        public Dictionary<string, string[]> subgoalPool = new Dictionary<string, string[]>();
        public int numberOfStages;
		public string message;

        int stageIndex = -1;

        // function to get current stageIndex
        public int GetCurrentStageIndex()
        {
            return stageIndex;
        }

        // function to set current stage index
        public void SetCurrentStageIndex(int index)
        {
            stageIndex = index;
        }

        // get current visual stage object for current index
        public VisualStageObject GetStageByIndex(int index)
        {
            if (index >= 0 && index < visualStages.Length)
            {
                return visualStages[index];
            }
            return null;
        }

        // get list of subgoal names for given index position
        public string[] GetSubgoalNames(int index)
        {
            return subgoalMap.ContainsKey(index) ? subgoalMap[index] : null;
        }

        // get set of object name for subgoals
        public string[] GetSubgoalObjectNames(int index)
        {
            HashSet<string> objectNames = new HashSet<string>();

            var names = GetSubgoalNames(index);
            if (names != null)
            {
                foreach (var subgoal in names)
                {
                    foreach (var on in subgoalPool[subgoal])
                    {
                        objectNames.Add(on);
                    }
                }
            }
            return objectNames?.ToArray();
        }

        public int[] GetStagesBySubgoal(string subgoalKey)
        {
            List<int> stages = new List<int>();
            foreach (var sm in subgoalMap)
            {
                if (sm.Value.Contains(subgoalKey))
                {
                    stages.Add(sm.Key);
                }
            }
            return stages?.ToArray();
        }

        // Retrieving the image of the object from the Visualisation file
        public string FetchImageString(string key)
        {
            if (imageTable.ContainsKey(key))
            {
                return imageTable[key];
            }
            return null;
        }

        // Get the next stage of the animation
        public VisualStageObject NextStage()
        {
            if (stageIndex + 1 < visualStages.Length)
            {
                return visualStages[++stageIndex];
            }
            else
            {
                return null;
            }
        }

        // Get the previous stage of the animation
        public VisualStageObject PreviousStage()
        {
            if (stageIndex > 0)
            {
                return visualStages[--stageIndex];
            }
            else
            {
                return null;
            }
        }

        // Get the final stage of the animation
        public VisualStageObject FinalStage()
        {
            return visualStages[visualStages.Length - 1];
        }

        // Get the initial stage of the animation
        public VisualStageObject ResetStage()
        {
            stageIndex = -1;
            return NextStage();
        }

        // function to check the current index is final stage
        public bool IsFinalStage()
        {
            return stageIndex == visualStages.Length - 1;
        }

    }
}
