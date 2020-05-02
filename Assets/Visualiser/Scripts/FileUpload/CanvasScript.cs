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

using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

// Script for Opening file panel and read file in built using the Java script plugin.
public class CanvasScript : MonoBehaviour
{
    [DllImport("__Internal")]
    static extern void UploaderCaptureClick(string extensionptr);

    [DllImport("__Internal")]
    private static extern void PasteHereWindow();
    
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
    // accept dropped files
    public void DropMultipleFiles(string json)
    {
        var file = FileData.CreateFromJSON(json);
        this.type = file.type;
        
        textfield = GameObject.Find(type + "Text").GetComponent<InputField>();
        textfield.text = file.name;
        string data = file.data;

        //Assigning file to corresponding variable and showing file name on UI
        setFileData(data);
    }

    public void DropSingleFile(string json)
    {
        var file = FileData.CreateFromJSON(json);;

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
            Debug.Log(target.gameObject.name);
            if(target.gameObject.name.Equals("DomainText") | target.gameObject.name.Equals("DomainButton")) {
                this.type = "Domain";
            } else if (target.gameObject.name.Equals("ProblemText") | target.gameObject.name.Equals("ProblemButton")) {
                this.type = "Problem";
            } else if (target.gameObject.name.Equals("AnimationText") | target.gameObject.name.Equals("AnimationButton")) {
                this.type = "Animation";
            // should change this component name to a unique one
            } else if (target.gameObject.name.Equals("UploadFields")) {
                this.type = "visualisationFile";
            }
        }

        textfield = GameObject.Find(type + "Text").GetComponent<InputField>();
        textfield.text = file.name;
        string data = file.data;
        
        //Assigning file to corresponding variable and showing file name on UI
        setFileData(data);
    }

    private void setFileData(string data) {
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

        if (type == "Animation"){
            UploaderCaptureClick(".pddl");
        } else if (type == "visualisationFile"){
            UploaderCaptureClick(".vfg");
        } else if (type=="Plan"){
            UploaderCaptureClick(".txt,.pddl");
        }else {
            UploaderCaptureClick(".pddl");
        }
    }

    [System.Serializable]
    public class FileData
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
    /* (Apr 23, 2020) Drag and Drop update */
}