using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlanVisualizerArchitecture.Entity.ScriptInfoPack;
using UnityEngine.UI;

public class Objects : MonoBehaviour {
	private List<Script_ObjectItem> objectsitems= new List<Script_ObjectItem>(10);
	private int objectnum = 0;
	// Use this for initialization
	void Start () {
		gameObject.GetComponent<Dropdown>().onValueChanged.AddListener(changetext);
		for (int i = 1; i < this.GetComponent<Dropdown>().options.Count;i++){
			Script_ObjectItem tempobjectsitems = new Script_ObjectItem();
			tempobjectsitems.name = this.GetComponent<Dropdown> ().options.ToArray () [i].ToString();
			tempobjectsitems.rules = null;
			objectsitems.Add (tempobjectsitems);
			objectnum++;
		}
	}

	public List<Script_ObjectItem> getObjectItems(){
		return this.objectsitems;
	}
	public void changetext(int index){
		GameObject ObjectRule = GameObject.Find ("ObjectRules");
		if (index != 0) {
			if (objectsitems [index - 1].rules != null)
				ObjectRule.GetComponent<InputField> ().text = objectsitems [index - 1].rules;
			else
				ObjectRule.GetComponent<InputField> ().text = "";
		} else {
			ObjectRule.GetComponent<InputField> ().text = "";
		}
	}
	public void addObject(GameObject inputfield){
		List<string> newoption = new List<string>();
		string newobject = inputfield.GetComponent<InputField> ().text;
		if (newobject != null) {
			newoption.Add (newobject);
			gameObject.GetComponent<Dropdown>().AddOptions(newoption);
			Script_ObjectItem newscrip_obj = new Script_ObjectItem();
			newscrip_obj.name = newobject;
			newscrip_obj.rules = null;
			this.objectsitems.Add (newscrip_obj);
			objectnum++;
		}
	}
	public void removeObject(GameObject inputfield){
		Dropdown droplist = gameObject.GetComponent<Dropdown> ();
		int removeat = droplist.value;
		if (removeat == (droplist.options.Count - 1)) {
			gameObject.GetComponent<Dropdown> ().value = removeat - 1;
			inputfield.GetComponent<InputField> ().text = objectsitems [removeat - 2].rules;
		} else {
			gameObject.GetComponent<Dropdown> ().value = removeat + 1;
			inputfield.GetComponent<InputField> ().text = objectsitems [removeat].rules;
		}
		objectsitems.RemoveAt ( removeat - 1);
		gameObject.GetComponent<Dropdown> ().options.RemoveAt (removeat);
		gameObject.GetComponent<Dropdown> ().RefreshShownValue ();
	}
}
