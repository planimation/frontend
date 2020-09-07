using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlanVisualizerArchitecture.Entity.ScriptInfoPack;
using UnityEngine.UI;

public class Inputfield : MonoBehaviour {
	private List<Script_PredicateItem> predicates = new List<Script_PredicateItem>(10);
	private List<Script_ObjectItem> constantObjects = new List<Script_ObjectItem>(10);
	private GameObject Object;
	//private GameObject ConstantObject;
	void Start(){
		gameObject.GetComponent<InputField>().onEndEdit.AddListener(endEdit);
	}
	public void endEdit(string text){
		if (this.name.Equals ("PredicateRules")) {
			this.Object = GameObject.Find ("Predicates");
			this.predicates = Object.GetComponent<predicates> ().getpredicateItems ();
			int value = this.Object.GetComponent<Dropdown> ().value - 1;
			if (value >= 0)
				predicates [value].rules = text;
		}
		if (this.name.Equals ("ObjectRules")) {
			this.Object = GameObject.Find ("Constants");
			this.constantObjects = Object.GetComponent<Objects> ().getObjectItems ();
			int value = this.Object.GetComponent<Dropdown> ().value - 1;
			if (value >= 0)
				constantObjects [value].rules = text;
		}

	}
}
