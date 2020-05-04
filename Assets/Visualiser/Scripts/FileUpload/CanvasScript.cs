///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/*
 * 
 * Purpose: The file has fucntionality to Upload files to server
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
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using UnityEngine.UI;

/* (Apr 23, 2020) Drag and Drop update */
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
/** (Apr 23, 2020) Drag and Drop update **/

// Script for Opening file panel and read file in built using the Java script plugin.
public class CanvasScript : MonoBehaviour
{
    [DllImport("__Internal")]
    static extern void UploaderCaptureClick(string extensionptr);

    [DllImport("__Internal")]
    private static extern void PasteHereWindow();

    /* (Apr 23, 2020) Drag and Drop update */
    // call javascript function through WebGL
    [DllImport("__Internal")]
    private static extern void UploadPDDLFile();

    [DllImport("__Internal")]
    private static extern void UploadVFGFile();
    /** (Apr 23, 2020) Drag and Drop update **/
    
    InputField textfield;
    string type;
    string name;



    // The input file models are strictly checked using IEnumerator
    IEnumerator LoadTexture(string url)
    {
        WWW file = new WWW(url);
        yield return file;
        string data = file.text;
        textfield = GameObject.Find(type + "Text").GetComponent<InputField>();
        textfield.text = name;
        //Assigning file to corresponding variable and showing file name on UI

        if (type == "Domain")
        {
            ScenesCoordinator.Coordinator.setDomain(data);

        }
        else if (type == "Problem")
        {
            ScenesCoordinator.Coordinator.setProblem(data);

        }
        else if (type == "Animation")
        {
            ScenesCoordinator.Coordinator.setAnimation(data);

        }else if (type == "Plan")
        {
            ScenesCoordinator.Coordinator.setPlan(data);

        }
        else if (type == "visualisationFile")
        {
            ScenesCoordinator.Coordinator.PushParameters("Visualisation", data);

        }
        
    }

    //Recieved call from Javascript code
    void FileSelected(string url)
    {

        StartCoroutine(LoadTexture(url));
    }
    //Getting the file name
    void SetFileName(string name)
    {
        this.name = name;
    }
    //Assign customsolver 
    public void setCustomSolver()
    {
        
        string solverAddress;
        solverAddress = GameObject.Find("CustomSolverInput").GetComponent<InputField>().text;
        ScenesCoordinator.Coordinator.setCustomSolver(solverAddress);
    }

    public void onPastedbutton()
    {
#if UNITY_EDITOR
#else
    PasteHereWindow();
#endif
    }
    //Paste text from pop up prompt window
    public void GetPastedText(string newpastedtext)
    {
        GameObject.Find("CustomSolverInput").GetComponent<InputField>().text = newpastedtext;
    }
    
    //trigger open file panel
    public void OnButtonPointerDown(string type)
    {
        this.type = type;
#if UNITY_EDITOR
        string path;
        if (type == "Animation"){
            path = UnityEditor.EditorUtility.OpenFilePanel("Open image","","pddl");
        }else if (type == "visualisationFile"){
            path = UnityEditor.EditorUtility.OpenFilePanel("Open image","","vfg");
        }else if (type=="Plan"){
            path = UnityEditor.EditorUtility.OpenFilePanel("Open image", "", "");
        }
        else {
            path = UnityEditor.EditorUtility.OpenFilePanel("Open image","","pddl");
        }
        if (!System.String.IsNullOrEmpty (path)){
            SetFileName(path.Split('/')[path.Split('/').Length-1]);
            FileSelected ("file:///" + path);
        }
#else
        if (type == "Animation"){
            UploaderCaptureClick(".pddl");
        } else if (type == "visualisationFile"){
            UploaderCaptureClick(".vfg");
        } else if (type=="Plan"){
            UploaderCaptureClick(".txt,.pddl");
        }else {
            UploaderCaptureClick(".pddl");
        }
#endif
    }

    /* (Apr 23, 2020) Drag and Drop update */
    const string DOMAIN = "Domain";
    const string PROBLEM = "Problem";
    const string ANIMATION = "Animation";
    const string VFG = "visualisationFile";
    const string PLAN = "Plan";
    const string TEXT = "Text";
    const string BUTTON = "Button";
    const string PARAM_VFG = "Visualisation";

    public void CheckCurrentScene() {

        string sceneName = SceneManager.GetActiveScene().name;
        
        // upload page for Domain/Problem/AnimationProfile files
        if(sceneName.Equals("Start")) {
            UploadPDDLFile();
        }
        
        // upload page for a VFG file
        else if(sceneName.Equals("VFGUploader")) {
            UploadVFGFile();
        }
    }

    // set each file data
    public void DropMultipleFiles(string json)
    {
        var file = FileData.CreateFromJSON(json);
        this.type = file.type;
        
        //Assigning file to corresponding variable and showing file name on UI
        setFileData(file);
    }

    // detect dropped position and set file data
    public void DropSingleFile(string json)
    {
        var file = FileData.CreateFromJSON(json);

        /* [note]
         * mousePosition = camera/screen coordinate
         * objectPosition = world/global coordinate
         *
         * Adopted mousePosition this time.
         */
        float x = float.Parse(file.x);
        float y = float.Parse(file.y);
        float z = 10.0f;
        Vector3 mousePos = new Vector3(x, y, z);
        //Vector3 objPos = Camera.main.ScreenToWorldPoint( mousePos );

        // detects current mouse position and find the colliding object
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        List<RaycastResult> results = new List<RaycastResult>();

        pointer.position = mousePos;
        EventSystem.current.RaycastAll(pointer, results);
        
        // ray tracing for object intersection
        foreach (RaycastResult target in results)
        {
            if(target.gameObject.name.Equals(DOMAIN + TEXT) | target.gameObject.name.Equals(DOMAIN + BUTTON)) {
                this.type = DOMAIN;
            } else if (target.gameObject.name.Equals(PROBLEM + TEXT) | target.gameObject.name.Equals(PROBLEM + BUTTON)) {
                this.type = PROBLEM;
            } else if (target.gameObject.name.Equals(ANIMATION + TEXT) | target.gameObject.name.Equals(ANIMATION + BUTTON)) {
                this.type = ANIMATION;
            // should change this component name to a unique one
            } else if (target.gameObject.name.Equals("UploadFields")) {
                this.type = VFG;
            }
        }
        
        //Assigning file to corresponding variable and showing file name on UI
        setFileData(file);
    }

    private void setFileData(FileData file) {

        textfield = GameObject.Find(type + TEXT).GetComponent<InputField>();
        textfield.text = file.name;
        string data = file.data;

        if (type == DOMAIN)
        {
            ScenesCoordinator.Coordinator.setDomain(data);

        }
        else if (type == PROBLEM)
        {
            ScenesCoordinator.Coordinator.setProblem(data);

        }
        else if (type == ANIMATION)
        {
            ScenesCoordinator.Coordinator.setAnimation(data);

        }else if (type == PLAN)
        {
            ScenesCoordinator.Coordinator.setPlan(data);

        }
        else if (type == VFG)
        {
            ScenesCoordinator.Coordinator.PushParameters(PARAM_VFG, data);

        }

        this.type = "";
    }

    // Custom object for json utility
    [System.Serializable]
    private class FileData
    {
        public string name;
        public string type;
        public string data;
        public string x;
        public string y;

        public static FileData CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<FileData>(jsonString);
        }
    }
    /** (Apr 23, 2020) Drag and Drop update **/
}