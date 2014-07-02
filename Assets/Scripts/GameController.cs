using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Xml;

public class GameController : MonoBehaviour {

	public GameObject player;
	public GameObject cameraContainer;
	public GameObject aim;
	public GameObject GUILabel;//テスト用

	public int currentStage = 0;

	//private List<EnemyData> enemyData = new List<EnemyData>();
	private List<Stage> stages = new List<Stage>();
	private List<Wave> waves = new List<Wave>();
	
	/*
	struct EnemyData {
		public float initX;
		public float initY;
		public float targetX;
		public float targetY;
		public EnemyData(float ix, float iy, float tx, float ty){
			initX = ix;
			initY = iy;
			targetX = tx;
			targetY = ty;
		}
	}
	*/

	// Use this for initialization
	void Start () {
		TextAsset xml = Resources.Load("enemy") as TextAsset;
		XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
		xmlDoc.LoadXml(xml.ToString ()); // load the file.
		//Resources.Load ("enemy.xml");
		XmlNodeList stageNodeList = xmlDoc.SelectNodes("stages/stage");
		for(int i=0; i<stageNodeList.Count; i++){
			stages.Add(new Stage(stageNodeList[i].Attributes["name"].Value));
			XmlNodeList waveNodeList = xmlDoc.SelectNodes("stages/stage/wave");
			for(int j=0; j<waveNodeList.Count; j++){
				XmlNodeList enemyNodeList = waveNodeList[j].SelectNodes("enemy");
				List<EnemyData> enemyList = new List<EnemyData>();
				for(int k=0; k<enemyNodeList.Count; k++){
					//Debug.Log (enemyNodeList[j].Attributes["ix"].Value);
					XmlNode currentEnemyNode = enemyNodeList[k];
					enemyList.Add (new EnemyData(currentEnemyNode.Attributes["name"].Value, float.Parse(currentEnemyNode.Attributes["ix"].Value), float.Parse(currentEnemyNode.Attributes["iy"].Value), float.Parse(currentEnemyNode.Attributes["x"].Value), float.Parse(currentEnemyNode.Attributes["y"].Value)));
				}
				waves.Add (new Wave(enemyList));
			}
		}

		aim.renderer.enabled = false;
		iTween.RotateBy(cameraContainer, iTween.Hash("x", 0.0f, "y", 1.0f, "z", 0.0f, "time", 2.5f, "delay", 0.0f));
		iTween.MoveTo(cameraContainer, iTween.Hash("position", new Vector3(0, 3.5f, 0), "time", 2.5f, "delay", 2.5f, "oncomplete", "CameraMoveComplete", "oncompletetarget", gameObject));
		iTween.RotateTo(cameraContainer, iTween.Hash("x", 90.0f, "y", 0.0f, "z", 0.0f, "time", 2.0f, "delay", 2.5f));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void StartStage(){

	}

	void WaveClear(){
		waves.RemoveAt(0);//リストから現在のWaveを削除
		if(waves.Count == 0){
			StartCoroutine("StageClear");
		}else{
			NextWave();
		}
	}

	IEnumerator GameOver () {
		GUILabel.guiText.text = "Gamw Over";
		yield return new WaitForSeconds(3.0f);
		Application.LoadLevel("Title");
	}

	IEnumerator StageClear () {
		Debug.Log("StageClear!!");
		if(currentStage > stages.Count - 1){
			GUILabel.guiText.text = "Stage Clear !!";
			Debug.Log ("NextStage");
			yield return new WaitForSeconds(3.0f);
			//NextStage();
		}else{
			GUILabel.guiText.text = "All Stage Clear !!";
			yield return new WaitForSeconds(3.0f);
			Application.LoadLevel("Title");
		}
	}

	void CameraMoveComplete () {
		player.GetComponent<Player>().enabled = true;
		//GameObject wave = (GameObject)GameObject.Instantiate(wavePrefab);
		//wave.SendMessage ("SetData", enemyData);
		//wave.GetComponent(Wave).enabled = true;
		aim.renderer.enabled = true;

		NextWave();//最初のWave開始

	}

	void NextWave(){
		//Debug.Log ("NextWave : 残り " + waves.Count);
		//wavesリストから最初のひとつを取り出してcurrentWaveとし、リストからは削除する（shift）
		Wave currentWave = waves[0];
		currentWave.GenerateEnemy();
	}

	void NextStage(){
		currentStage++;
	}
}