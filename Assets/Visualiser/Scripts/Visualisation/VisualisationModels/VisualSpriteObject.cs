///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/*
 * 
 * Purpose: The file has fucntionality to set each animation object graphic properties 
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
	/* This class contains information of an individual object at a stage in the Visualisation file,
	 * this including its name, image, and location*/
    [Serializable]
    public class VisualSpriteObject
    {
        public string name;
        public string prefabimage;
        public Color color;
        public bool showname;
		public bool showlabel;
		public string label;
        public float minX;
        public float maxX;
        public float minY;
        public float maxY;
        public float rotate;
        public int depth;
        public int transferType;
		// checking if the given VisualSpriteObject is same as this class
        // mapper class and make sure the object is valid
        public bool ContentsEqual(VisualSpriteObject vso)
        {
            return              name == vso.name 
                              && prefabimage == vso.prefabimage
                              && Mathf.Approximately(minX, vso.minX) && Mathf.Approximately(maxX, vso.maxX)
                              && Mathf.Approximately(minY, vso.minY) && Mathf.Approximately(maxY, vso.maxY)
                              && color == vso.color
                              && depth == vso.depth
							  && label == vso.label;
        }
    }
}
