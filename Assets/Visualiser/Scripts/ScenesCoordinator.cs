///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/*
 *
 * Purpose: The scenes coordinator manages parameters that are passing through different scenes
 * Authors: Tom, Collin, Hugo and Sharukh
 * Date: 14/08/2018
 * Reviewers: Sharukh, Gang and May
 * Review date: 10/09/2018
 *
 * /
 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  */

/* The scenes coordinator manages parameters that are passing through different scenes, including
 * all the uploaded files and the visualisation file*/
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.IO;
using System.Text;
using Visualiser;

public class ScenesCoordinator : MonoBehaviour
{
    // External web config file (Sep 14, 2020)
    [DllImport("__Internal")]
    private static extern string GetUploadApi();

    public static ScenesCoordinator Coordinator;
    Dictionary<string, object> sceneParameters;
    private string customSolverDomain;
    private string domaintxt;
    private string problemtxt;
    //" " means no plan has been uploaded
    private string plantxt=" ";
	  private string animationprofile;

    // Sep 15 2020 Zhaoqi Fang)
    // if UNITY_STANDALONE, get vfg from CLI, passi vfg and redirect to Visualisation Scene
    // Comment the following part of code if using WEBGL
    //private void Start()
    //{
    //    var args = System.Environment.GetCommandLineArgs();
    //    var reader = new StreamReader(args[1]);
    //    string vfg = reader.ReadLine();
    //    reader.Close();
    //    Debug.Log("vfg is:\n" + vfg);
    //    Coordinator.PushParameters("Visualisation", vfg);
    //    SceneManager.LoadScene("Visualisation");
    //}
    // Comment the above part of code if using WEBGL
    private void Awake()
    {
        if (Coordinator == null)
        {
            Coordinator = this;
        }
        DontDestroyOnLoad(gameObject);

        sceneParameters = new Dictionary<string, object>();
    }

    // Retrieving the visualisation file
    public object FetchParameters(string sceneName)
    {
        return sceneParameters[sceneName];
    }

    // @Sep 22, 2020 Zhaoqi Fang) check if parameters exists for file Download
    public bool CheckParameters(string sceneName)
    {
        if (sceneParameters.ContainsKey(sceneName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // (Sep 23, 2020 Zhaoqi Fang) remove parameters
    public void RemoveParameters(string sceneName)
    {
        if (sceneParameters.ContainsKey(sceneName))
        {
            sceneParameters.Remove(sceneName);
        }
    }

    // Adding the visualisation file to the visualiser scene
    public void PushParameters(string sceneName, object parameters)
    {
        sceneParameters.Add(sceneName, parameters);
    }

    // Interface for other objects to use
    public void uploadVF(){
    	SceneManager.LoadScene ("Visualisation");
    }
    public void planimationPlugin()
    {
        SceneManager.LoadScene("planimationPlugin");
    }
    // Interface for other objects to use
    public void uploadallfile(){
    	StartCoroutine (generateVisualiser ());
    }

    // Make a POST request to the server for the visualiser file
    IEnumerator generateVisualiser(){
    	//generate a unique boundary
    	byte[] boundary = UnityWebRequest.GenerateBoundary();
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
    	formData.Add( new MultipartFormDataSection("domain",domaintxt ));
    	formData.Add( new MultipartFormDataSection("problem",problemtxt ));
    	formData.Add( new MultipartFormDataSection("animation",animationprofile ));
      formData.Add(new MultipartFormDataSection("plan", plantxt));
      formData.Add(new MultipartFormDataSection("url", customSolverDomain));
      //serialize form fields into byte[] => requires a bounday to put in between fields
      byte[] formSections = UnityWebRequest.SerializeFormSections(formData, boundary);
      /* upload test for drag and drop function */

      string port = GetUploadApi();
      Debug.Log("testing the get upload api method" + port);
      UnityWebRequest www = UnityWebRequest.Post(port, formData);
      // download sequential PNG, add javascript lib in index.html and changes in downloadMovie.js

      // UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:8000/upload/pddl", formData);
       //UnityWebRequest www = UnityWebRequest.Post("https://planning-visualisation-solver.herokuapp.com/upload/pddl", formData);
      //UnityWebRequest www = UnityWebRequest.Post("/upload/pddl", formData);
      // download sequential PNG, add javascript lib in index.html and changes in downloadMovie.js

      www.uploadHandler =  new UploadHandlerRaw(formSections);
    	www.SetRequestHeader("Content-Type", "multipart/form-data; boundary="+ Encoding.UTF8.GetString(boundary));
    	yield return www.SendWebRequest();
    	// Showing error scene if plan are not found or experiencing network error
    	if(www.isNetworkError || www.isHttpError) {
            SceneManager.LoadScene("NetworkError");
            Debug.Log(www.error);
		}
		else {
			//Debug.Log("Form upload complete!");
			Coordinator.PushParameters ("Visualisation", www.downloadHandler.text);
			SceneManager.LoadScene ("Visualisation");
		}
	}
    // (Sep 21, 2020 Zhaoqi Fang) send vfg and required file format to backend
    public void SendDownloadRequest(string fileType)
    {
        StartCoroutine(GenerateDownloader(fileType));
    }

    IEnumerator GenerateDownloader(string fileType)
    {
        // generate a unique boundary
        byte[] boundary = UnityWebRequest.GenerateBoundary();
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        string vfg = FetchParameters("Visualisation") as string;
        formData.Add(new MultipartFormDataSection("vfg", vfg));
        formData.Add(new MultipartFormDataSection("fileType", fileType));
        // serialize form fileds into byte[] => requires a boundary to put in between fields
        byte[] formSections = UnityWebRequest.SerializeFormSections(formData, boundary);

        UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:8000/downloadVisualisation", formData);

        www.uploadHandler = new UploadHandlerRaw(formSections);
        www.SetRequestHeader("Content-Type", "multipart/form-data; boundary=" + Encoding.UTF8.GetString(boundary));
        yield return www.SendWebRequest();
        // Showing error scene if vfg error or experiencing network error
        if (www.isNetworkError || www.isHttpError)
        {
            SceneManager.LoadScene("NetworkError");
            Debug.Log(www.error);
        }
        else
        {  
            Coordinator.PushParameters("DownloadPlanimation", www.downloadHandler.data);
            
            
        }
    }

    // Storing the doamin file in the coordinator
    public void setDomain(string domain){
    	this.domaintxt = domain;
    }
    // Storing the problem file in the coordinator
    public void setProblem(string problem){
    	this.problemtxt = problem;
    }
    // Storing the aniamation profile in the coordinator
    public void setAnimation(string animation){
    	this.animationprofile = animation;
    }
    // Storing the plan file in the coordinator
    public void setPlan(string plan)
    {
        this.plantxt = plan;
    }
    // Get custom solver address and store it
    public void setCustomSolver(string customSolver)
    {
        this.customSolverDomain = customSolver;
    }

}
