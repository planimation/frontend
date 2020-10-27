
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/*
 *  
 * Purpose: The controller of the entire visualisation [********Important file**********]
 * Authors: Tom, Collin, Hugo and Sharukh
 * Date: 14/08/2018
 * Reviewers: Sharukh, Gang and May
 * Review date: 10/09/2018
 * 
 * /
 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  */



using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;
using Debug = UnityEngine.Debug;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections;

using System.IO;

/* (May 17, 2020) Movie/Gif update */
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
/** (May 17, 2020) Movie/Gif update **/

namespace Visualiser
{
    /*
     * The controller of the entire visualisation
     * It manages the solution file and all visual objects in every stage
     */
    public class VisualiserController : MonoBehaviour
    {
        //Download function
        [DllImport("__Internal")]
        private static extern void Download(string textptr, string fileTypeptr, string fileNameptr);

        /* (May 17, 2020) Download Movie update */
        // call javascript function through WebGL
        [DllImport("__Internal")]
        private static extern void StartRecording();

        [DllImport("__Internal")]
        private static extern void LockScreen();

        [DllImport("__Internal")]
        private static extern void OutputGIF();

        [DllImport("__Internal")]
        private static extern void OutputWebM();

        [DllImport("__Internal")]
        private static extern void OutputMP4();
        /** (May 17, 2020) Download Movie update **/

        // Static readonly fileds
        readonly static Color SubgoalImplementedColor = new Color(0f, 0.66666667f, 0.10980392f);
        readonly static Color StepButtonHighlightedColor = new Color(0.5613208f, 0.826122f, 1f);
        readonly static Color DefaultColor = Color.white;

        // Editor interface
        public Transform SubgoalPanel;
        public GameObject AniFrame;
        public GameObject InforScreen;
        public GameObject StepCostScreen;
        public GameObject TotalCostScreen;
        public GameObject Speedbar;
        public GameObject SubgoalProgressText;
        public Transform StepPanel;
        public ScrollRect StepScrollRect;
        public ScrollRect SubgoalScrollRect;
        public Image PlayButtonSprite;

        /* (May 17, 2020) Download Movie update */
        public GameObject DownloadPanelFull;
        public GameObject DownloadPanelVFG;
        public GameObject BrowserAlertPanel;
        /** (May 17, 2020) Download Movie update **/

        // Sprite objects
        Sprite PlaySprite;
        Sprite PauseSprite;

        // Private fields
        ScenesCoordinator coordinator = ScenesCoordinator.Coordinator; // Manages scenes
        VisualSolutionObject visualSolution; // Contains all the information of a solution
        readonly Dictionary<string, GameObject> spritePool = new Dictionary<string, GameObject>(); // Visible objects pool
        readonly List<Dropdown> subgoalDropdowns = new List<Dropdown>();
        readonly List<Button> stepButtons = new List<Button>();
        Button highlightingButton;
        bool playing;   // Indicates if palying animation
        int storedStage; // Stores the stage index before jumping to the final stage
        string vf;

        /* (May 17, 2020) Download Movie update */
        string filetype;
        bool waitReset;
        /** (May 17, 2020) Download Movie update **/
        
        
        // Intialization function 
        void Start()
        {
            // Reads visualisation file data
		try{
            var parameters = coordinator.FetchParameters("Visualisation") as string;
            vf = parameters;
            // Creates a visual solution
            visualSolution = JsonUtility.FromJson<VisualSolutionObject>(parameters);
			if (visualSolution.message != ""){
				coordinator.PushParameters("NetworkError",visualSolution.message);
				SceneManager.LoadScene("NetworkError");
				return;
			}
            // ------- json parsing work around
            var jo = JsonConvert.DeserializeObject<Dictionary<string, object>>(parameters);
            var smjo = JObject.FromObject(jo["subgoalMap"]);
            var spjo = JObject.FromObject(jo["subgoalPool"]);
            var smjs = smjo.ToString(Formatting.None);
            var spjs = spjo.ToString(Formatting.None);
            var sm = JsonConvert.DeserializeObject<SubgoalMapDictionary>(smjs);
            var sp = JsonConvert.DeserializeObject<SubgoalPoolDictionary>(spjs);
            for (var i = 0; i < sm.m_keys.Length; ++i)
            {
                visualSolution.subgoalMap.Add(sm.m_keys[i], sm.m_values[i]);
            }
            for (var i = 0; i < sp.m_keys.Length; ++i)
            {
                visualSolution.subgoalPool.Add(sp.m_keys[i], sp.m_values[i]);
            }
            // JSON parsing end

            // Intialize sprite objects
            PlaySprite = Resources.Load<Sprite>("PlaySprite");
            PauseSprite = Resources.Load<Sprite>("PauseSprite");

            // Renders the first frame of the visualisation
            var visualStage = visualSolution.NextStage();
			
	            RenderSubgoals();
	            RenderSteps();
	            RenderFrame(visualStage);
			}catch (Exception e){
				//SceneManager.LoadScene("NetworkError");
			}
        }


        // This function is used to render subgoals from the solutionobject
        public void RenderSubgoals()
        {
            foreach (var subgoal in visualSolution.subgoalPool)
            {
                var dropdownPrefab = Resources.Load<GameObject>("SubgoalDropDown");
                var subgoalDropdown = Instantiate(dropdownPrefab);
                subgoalDropdown.transform.SetParent(SubgoalPanel, false);

                var subgoalText = subgoalDropdown.GetComponentInChildren<Text>();
                subgoalText.text = subgoal.Key;

                var dropdownComp = subgoalDropdown.GetComponent<Dropdown>();
                dropdownComp.name = subgoal.Key;
                var emptyOption = new Dropdown.OptionData(string.Empty);
                dropdownComp.options.Add(emptyOption);
                var stages = visualSolution.GetStagesBySubgoal(subgoal.Key);
                foreach (var stage in stages)
                {
                    var optionData = new Dropdown.OptionData("Step " + stage);
                    dropdownComp.options.Add(optionData);
                }
                dropdownComp.onValueChanged.AddListener((int index) =>
                {
                    if (index != 0)
                    {
                        var stageText = dropdownComp.options[index].text;
                        var stage = int.Parse(stageText.Replace("Step ", string.Empty));
                        dropdownComp.value = 0;
                        subgoalText.text = subgoal.Key;

                        Pause();
                        PresentStageByIndex(stage);
                    }
                });

                subgoalDropdowns.Add(dropdownComp);

                var spText = SubgoalProgressText.GetComponent<Text>();
                spText.text = string.Format("{0}/{1}", 0, visualSolution.subgoalPool.Count);
            }
        }

        //  This function is used to render steps from the solutionobject
        public void RenderSteps()
        {
            var stepButtonPrefab = Resources.Load<GameObject>("StepButton");
            var i = 0;
            foreach (var stage in visualSolution.visualStages)
            {
                var index = i;

                var stepButtonObj = Instantiate(stepButtonPrefab);
                stepButtonObj.transform.SetParent(StepPanel, false);
                var stepText = stepButtonObj.GetComponentInChildren<Text>();
                stepText.text = $"{i++}. {stage.stageName}";
                var stepButton = stepButtonObj.GetComponent<Button>();
                stepButton.onClick.AddListener(() =>
                {
                    Pause();
                    PresentStageByIndex(index);
                });
                stepButtons.Add(stepButton);
            }
        }

        #region UI event handlers
        // UI event handler: Presents the contents of the final stage
        public void PresentFinalGoal()
        {
            storedStage = visualSolution.GetCurrentStageIndex();
            var visualStage = visualSolution.FinalStage();
            TryRenderFrame(visualStage);
        }

        // UI event handler: Presents the stored stage before jumping to the final stage
        public void PresentStoredStage()
        {
            PresentStageByIndex(storedStage);
        }

        // Used by other events, presents a specific stage by index
        public void PresentStageByIndex(int index)
        {
            var stage = visualSolution.GetStageByIndex(index);
            visualSolution.SetCurrentStageIndex(index);
            TryRenderFrame(stage);
        }

        // UI event handler: Presents the contents of next stage
        public void PresentNextStage()
        {
            var visualStage = visualSolution.NextStage();
            TryRenderFrame(visualStage);
        }

        // UI event handler: Presents the contents of previous stage
        public void PresentPreviousStage()
        {
            var visualStage = visualSolution.PreviousStage();
            TryRenderFrame(visualStage);
        }

        // UI event handler: Cleans up visualisation states and goes back to the first stage
        public void ResetStage()
        {
            Pause();
            var visualStage = visualSolution.ResetStage();
            TryRenderFrame(visualStage);
        }

        // UI event handler: Plays visualisation (animation)
        public void PlayPause()
        {
            if (playing)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        void Play()
        {
            playing = true;
            PlayButtonSprite.sprite = PauseSprite;
        }

        void Pause()
        {
            playing = false;
            PlayButtonSprite.sprite = PlaySprite;
        }

	public void disableallstepbtn(GameObject steps){
			Button[] btnsteps = steps.GetComponentsInChildren<Button> ();
			foreach (var step in btnsteps) {
				step.interactable = false;
			}
	}
	public void enableallstepbtn(GameObject steps){
		Button[] btnsteps = steps.GetComponentsInChildren<Button> ();
		foreach (var step in btnsteps) {
			step.interactable = true;
		}
	}

        // UI event handler: Jumps to user manual page 
        public void Help()
        {
            var newURL = "https://planning-visualisation-solver.herokuapp.com/help/";
            Application.OpenURL (newURL);
        }
        #endregion


        // Unity built-in method, it will be fired in every frame
        void Update()
        {   
            // Plays animation
            if (playing && AreAllAnimationsFinished())
            {   
                
                //Debug.Log("captured?  " + name);

                if (visualSolution.IsFinalStage())
                {
                    Pause();
                    
                    /* (May 27, 2020) Movie download update */
                    if(this.filetype != null) 
                    {
                        if(this.filetype == "gif") 
                        {
                            OutputGIF();
                        } 
                        else if(this.filetype == "webm")
                        {
                            OutputWebM();
                        }
                        else if(this.filetype == "mp4")
                        {
                            OutputMP4();
                        }
                        this.filetype = null;
                    }
                    /* (May 27, 2020) Movie download update */
                }
                else
                {
                    PresentNextStage();
                }
            }
        }

        // Check fucntion to make sure the animation rendering is successful
        bool AreAllAnimationsFinished()
        {
            foreach (GameObject spriteobject in spritePool.Values)
            {
                if (spriteobject.GetComponent<SpriteController>().IsAnimating())
                {
                    return false;
                }
            }
            return true;
        }

        // Download fucntion call for vfg file
        public void DownloadVF()
        {
            Download(vf, "text/plain", "vf_out.vfg");
            /* (May 27, 2020) Movie download update */
            DownloadPanelFull.SetActive(false);
            DownloadPanelVFG.SetActive(false);
            /** (May 27, 2020) Movie download update **/
        }

        #region Stage Rendering
        // Renders a frame if it is not null
        void TryRenderFrame(VisualStageObject visualStage)
        {
            if (visualStage != null)
            {
                RenderFrame(visualStage);
            }
        }

        // Renders a frame (stage), all visible objects will be drawn on the screen in this process
        void RenderFrame(VisualStageObject visualStage)
        {
            //Get subgoal info
            var stageIndex = visualSolution.GetCurrentStageIndex();
            var subgoalNames = visualSolution.GetSubgoalNames(stageIndex);
            var subgoalObjectNames = visualSolution.GetSubgoalObjectNames(stageIndex);

            // Render step info of current stage
            UpdateInformationFrame(visualStage);
            // Highlight stage object for rendering
            UpdateStepPanelUI(stageIndex);
            // Update subgoal UI
            UpdateSubgoalUI(subgoalNames);
            // Update visual sprites.
            // Including create, update, remove visual sprites and manage subgoal status
            UpdateVisualSprites(visualStage, subgoalObjectNames);
        }

        // Update buttons highlighting status
        public void UpdateStepPanelUI(int index)
        {
            if (highlightingButton != null)
            {
                highlightingButton.GetComponent<Image>().color = DefaultColor;
            }
            highlightingButton = stepButtons[index];
            highlightingButton.GetComponent<Image>().color = StepButtonHighlightedColor;

            var position = 1 - (float)index / (stepButtons.Count - 1);
            StepScrollRect.verticalNormalizedPosition = position;
        }

        // Update step info of the visual stage
        void UpdateInformationFrame(VisualStageObject visualStage)
        {
            String stepInfo = visualStage.stageInfo;
            var stepInfoText = InforScreen.GetComponentInChildren<Text>();
            stepInfoText.text = string.IsNullOrEmpty(stepInfo) ?
                                    "No Step Informattion available" :
                                    stepInfo;
            String stepCost = visualStage.stepCost;
            var stepCostText = StepCostScreen.GetComponentInChildren<Text>();
            stepCostText.text = string.IsNullOrEmpty(stepCost) ?
                                    "No Cost available" :
                                    stepCost;
            String totalCost = visualStage.totalCost;
            var totalCostText = TotalCostScreen.GetComponentInChildren<Text>();
            totalCostText.text = string.IsNullOrEmpty(totalCost) ?
                                    "No Cost available" :
                                    totalCost;
        }

        // Update subgoal panel UI
        void UpdateSubgoalUI(string[] subgoalNames)
        {
            if (subgoalNames != null)
            {
                foreach (var subgoal in subgoalDropdowns)
                {
                    subgoal.GetComponent<Image>().color = subgoalNames.Contains(subgoal.name) ?
                        SubgoalImplementedColor :
                        DefaultColor;
                }
                var spText = SubgoalProgressText.GetComponent<Text>();
                spText.text = string.Format("{0}/{1}", subgoalNames.Length, visualSolution.subgoalPool.Count);
            }
            else
            {
                foreach (var subgoal in subgoalDropdowns)
                {
                    subgoal.GetComponent<Image>().color = DefaultColor;
                }
                var spText = SubgoalProgressText.GetComponent<Text>();
                spText.text = string.Format("{0}/{1}", 0, visualSolution.subgoalPool.Count);
            }
        }

        // Update visual sprites game object and manage subgoal status
        void UpdateVisualSprites(VisualStageObject visualStage, string[] subgoalObjectNames)
        {
            //Render all visual sprite objects of current visual stage
            foreach (var visualSprite in visualStage.visualSprites)
            {
                //Check existin
                if (spritePool.ContainsKey(visualSprite.name))
                {
                    var sprite = spritePool[visualSprite.name];
                    var controller = sprite.GetComponent<SpriteController>();
                    controller.UpdateState(visualSprite);
                }
                //Create a new sprite if it does not exist
                else
                {
                    GameObject sprite;
                    var imageKey = visualSprite.prefabimage;
                    var imageString = visualSolution.FetchImageString(imageKey);
                    sprite = imageString != null
                        // Render custom prefab image
                        ? UIVisualSpriteFactory.CreateCustom(imageString)
                        // Render built-in prefab
                        : UIVisualSpriteFactory.CreateBuiltIn(visualSprite.prefabimage);
                    // Set parent relationship
                    sprite.transform.SetParent(AniFrame.transform, false);
                    // Store in sprite pool
                    spritePool.Add(visualSprite.name, sprite);
                    // Initialise sprite controller and start presenting the object
                    var controller = sprite.GetComponent<SpriteController>();
                    controller.Init(visualSprite);
                    controller.Present();
                }
            }
            //Remove stored sprites if they are not longer existing
            if (spritePool.Count > visualStage.visualSprites.Length)
            {
                var existingSpriteKeys = from i in visualStage.visualSprites
                                         select i.name;
                var temps = new List<string>();
                foreach (var spriteKey in spritePool.Keys)
                {
                    if (!existingSpriteKeys.Contains(spriteKey))
                    {
                        temps.Add(spriteKey);
                    }
                }
                foreach (var temp in temps)
                {
                    var sprite = spritePool[temp];
                    var controller = sprite.GetComponent<SpriteController>();
                    controller.OnDestory += (sender, e) =>
                    {
                        foreach (Transform child in sprite.transform)
                        {
                            Destroy(child.gameObject);
                        }
                        Destroy(sprite);
                        spritePool.Remove(temp);
                    };
                    controller.DisapperAndDestory();
                }
            }

            // Set subgoals
            foreach (var sprite in spritePool)
            {
                var controller = sprite.Value.GetComponent<SpriteController>();
                var flag = subgoalObjectNames.Contains(sprite.Key);
                controller.SetSubgoal(flag);
            }
        }
        #endregion

        /* (May 17, 2020) Download Movie update */
        // activate download panel
        public void CheckBrowser(string json)
        {
            var click = ClickEvent.CreateFromJSON(json);
            float x = float.Parse(click.x);
        	float y = float.Parse(click.y);
        	float z = 10.0f;
	        Vector3 mousePos = new Vector3(x, y, z);

	        // detects current mouse position and find the colliding object
	        PointerEventData pointer = new PointerEventData(EventSystem.current);
	        List<RaycastResult> results = new List<RaycastResult>();

	        pointer.position = mousePos;
	        EventSystem.current.RaycastAll(pointer, results);
	        
	        // ray tracing for object intersection
	        foreach (RaycastResult target in results)
	        {
	            if(target.gameObject.name.Equals("DownloadButton")) {
	                var browser = bool.Parse(click.browser);
	                if(browser) {
	                	DownloadPanelFull.SetActive(!DownloadPanelFull.activeSelf);
		            } else {
		            	DownloadPanelVFG.SetActive(!DownloadPanelVFG.activeSelf);
	            	}
	                break;
	            }
	        }
        }

        // activate file format panel for valid browsers (Chrome/Firefox/Opera)
        public void OpenFileSelectorFull()
        {
            DownloadPanelFull.SetActive(true);
        }

        // activate file format panel for invalid browsers (other than above three)
        public void OpenFileSelectorVFG()
        {
            DownloadPanelVFG.SetActive(true);
        }

        // activate alert panel for invalid browsers
        public void OpenBrowserAlert()
        {
            BrowserAlertPanel.SetActive(true);
        }

        // deactivate alert panel for invalid browsers
        public void CloseBrowserAlert()
        {
            BrowserAlertPanel.SetActive(false);
        }
        
        // set up condition and start screen recording 
        public void RecordPlayback(string filetype)
        {   
            this.filetype = filetype;
            this.waitReset = true;
            // close panel and reset status
            DownloadPanelFull.SetActive(false);
            LockScreen();
            ResetStage();
            while (!IsInitialState());
            StartRecording();
            Invoke("Play", 0.5f);
        }

        // check if the state is at the beginning
        private bool IsInitialState()
        {   
            return visualSolution.PreviousStage() == null;
        }

        // Custom object for json utility
	    [System.Serializable]
	    private class ClickEvent
	    {
	        public string browser;
	        public string x;
	        public string y;

	        public static ClickEvent CreateFromJSON(string jsonString)
	        {
	            return JsonUtility.FromJson<ClickEvent>(jsonString);
	        }
	    }
        /** (May 17, 2020) Download Movie update **/
    }
}