using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour {
	ScenesCoordinator coordinator = ScenesCoordinator.Coordinator;
	// Use this for initialization
	void Start () {
		try{
			string error = coordinator.FetchParameters ("NetworkError") as string;
			GameObject.Find("Description").GetComponent<Text>().text = error;
		} catch (Exception e) {
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
