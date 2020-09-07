using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlanVisualizerArchitecture.Entity.ScriptInfoPack;
using UnityEngine.UI;

public class predicates : MonoBehaviour {
	private List<Script_PredicateItem> predicatesitems= new List<Script_PredicateItem>(10);
	// Use this for initialization
	void Start () {
		gameObject.GetComponent<Dropdown>().onValueChanged.AddListener(changetext);
		for (int i = 1; i < this.GetComponent<Dropdown>().options.Count;i++){
			Script_PredicateItem tempredict = new Script_PredicateItem();
			tempredict.name =this.GetComponent<Dropdown> ().options.ToArray () [i].ToString();
			tempredict.rules = null;
			predicatesitems.Add (tempredict);
		}
	}

	public List<Script_PredicateItem> getpredicateItems(){
		return this.predicatesitems;
	}
	public void changetext(int index){
		GameObject predicateRule = GameObject.Find ("PredicateRules");
		if (index != 0) {
			if (predicatesitems [index - 1].rules != null)
				predicateRule.GetComponent<InputField> ().text = predicatesitems [index - 1].rules;
			else
				predicateRule.GetComponent<InputField> ().text = "";
		} else {
			predicateRule.GetComponent<InputField> ().text = "";
		}
	}
}
