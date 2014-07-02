using UnityEngine;
using System.Collections;

public class ConfigGUI : MonoBehaviour {

	private bool use3Way = false;
	private bool useRapidShot = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		if(GUI.Button(new Rect(50.0f, 250.0f, 150.0f, 50.0f), "Back")){
			Application.LoadLevel("Title");
		}
		
		use3Way = GUI.Toggle(new Rect(30.0f, 30.0f, 100.0f, 30.0f), use3Way, "3way");
		useRapidShot = GUI.Toggle(new Rect(30.0f, 70.0f, 100.0f, 30.0f), useRapidShot, "Rapid");
	}
}
