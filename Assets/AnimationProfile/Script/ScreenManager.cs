///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/*
 * 
 * Purpose: Manage the all the scene transaction and destroying non-DetroyOnLoad gameobject
 * when resetting
 * Authors: Tom, Collin, Hugo and Sharukh
 * Date: 14/08/2018
 * 
 * /
 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices; // used for runtime javascript function

public class ScreenManager : MonoBehaviour
{
    // import javascript function, functions stored at plugins/OpenLink.jslib
    [DllImport("__Internal")]
    private static extern void OpenDemoLink();
    [DllImport("__Internal")]
    private static extern void OpenDocumentaionLink(); 
    
	// Use this for initialization, the SceneManager must be a non-DetroyOnLoad gameobject
    void Start()
    {
        DontDestroyOnLoad(this);
    }

	// Load the visualiser uploading scene 
    public void loadVisualiseScene()
    {
		Destroy (GameObject.Find ("Coordinator"));
        SceneManager.LoadScene("Start");
    }

	// Load the visualiser File uploading scene 
	public void loadVFGUploadScene()
	{
		Destroy (GameObject.Find ("Coordinator"));
		SceneManager.LoadScene("VFGUploader");
	}

    // Load the visualiser solution uploading scene 
    public void loadVisualiseSolutionScene()
    {
        Destroy(GameObject.Find("Coordinator"));
        SceneManager.LoadScene("StartSolution");
    }
    // Load the index page scene
    public void loadMainScene(){
		Destroy (GameObject.Find ("Coordinator"));
        SceneManager.LoadScene("Landing Page");
    }
	// Load the scene for playing animation
	public void loadAnimationScene()
	{
		SceneManager.LoadScene("Visualisation");
	}
	
    // Goto user documentation link
    public void gotoDocumentation()
    {
        OpenDocumentaionLink();
    }

    // Go to Github Demo page
    public void gotoDemoPage()
    {
        OpenDemoLink();
    }

}
