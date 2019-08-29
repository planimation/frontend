///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/*
 *
 * Purpose: Buildscript - run at build time. Ensures correct scenes are loaded.
 * Authors: Tom, Collin, Hugo and Sharukh
 * Date: 14/08/2018
 *
 * /
 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
public class Buildscript : MonoBehaviour {

  static void Build () {
    BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions ();
    buildPlayerOptions.scenes = new [] {
       "Assets/Visualiser/Scenes/Landing Page.unity",
            "Assets/Visualiser/Scenes/Start.unity",
            "Assets/Visualiser/Scenes/Visualisation.unity",
            "Assets/Visualiser/Scenes/NetworkError.unity",
            "Assets/Visualiser/Scenes/VFGUploader.unity",
            "Assets/Visualiser/Scenes/PlanimationPlugin.unity"
    };
    buildPlayerOptions.locationPathName = "build";
    buildPlayerOptions.target = BuildTarget.WebGL;
    buildPlayerOptions.options = BuildOptions.None;

    BuildReport report = BuildPipeline.BuildPlayer (buildPlayerOptions);
    BuildSummary summary = report.summary;

    if (summary.result == BuildResult.Succeeded) {
      Debug.Log ("Build succeeded: " + summary.totalSize + " bytes");
    }

    if (summary.result == BuildResult.Failed) {
      Debug.Log ("Build failed");
    }
  }
}
