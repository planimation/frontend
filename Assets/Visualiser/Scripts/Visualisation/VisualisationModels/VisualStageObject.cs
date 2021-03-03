
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/*
 * 
 * Purpose: The file datastructure of each stage and has stage information.
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

namespace Visualiser
{
    /* This class contains all the VisualSpriteObjects at this stage object 
     * datastructure for visual stage object
     */
    [Serializable]
    public class VisualStageObject
    {
        public VisualSpriteObject[] visualSprites;
        public string stageName;
        public string stageInfo;
    }


}