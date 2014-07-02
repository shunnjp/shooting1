using UnityEngine;
using System.Collections;

public class TitleGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		if(GUI.Button(new Rect(50.0f, 250.0f, 150.0f, 50.0f), "Start")){
			Application.LoadLevel("Main");
		}
		if(GUI.Button(new Rect(250.0f, 250.0f, 150.0f, 50.0f), "Config")){
			Application.LoadLevel("Config");
		}
	}
}
