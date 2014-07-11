using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wave {
	public GameObject wavePrefab;
	public GameObject enemyPrefab;
	//public GameObject[] enemies = new GameObject[2];
	public string[] enemies;

	private List<EnemyData> enemyList;//EnemyData構造体が格納される
	
	private int num;
	private int destroyed = 0;

	public Wave(List<EnemyData> l) {
		enemyList = l;
		/*
		enemies = new strin
		g[enemyList.Count];
		int counter = 0;
		foreach(EnemyData i in enemyList){
			enemies[counter] = i.name;
			counter++;
		}
		*/
	}

	/*
	void SetData(EnemyData d) {
		Debug.Log ("Wave.SetData");
		Debug.Log (d);
		GenerateEnemy();
	}
	*/

	public void GenerateEnemy(){
		num = enemyList.Count;
		foreach(EnemyData i in enemyList){
			Vector3 initPos = new Vector3(i.initX, i.initY, 0.0f);//上から見下ろしている視点のため、initYといいつつZにセット
			Vector3 targetPos = new Vector3(i.targetX, i.targetY, 0.0f);
			GameObject enemy = (GameObject)GameObject.Instantiate(Resources.Load ("Enemy/" + i.name), initPos, Quaternion.identity);
			enemy.GetComponent<Enemy>().wave = this;
			enemy.SendMessage("Show", targetPos);
		}

		/*
		Vector3 initPos = new Vector3(-6.0f, 0.0f, 4.0f);
		Vector3 targetPos = new Vector3(-4.0f, 0.0f, 2.0f);
		if(Random.Range(0, 2) == 0){
			//出現位置をランダムに決める
			
		}else{
			initPos.z = -4.0f;
			targetPos.z = -2.0f;
		}
		if(Random.Range(0, 2) == 0){
			//出現位置をランダムに決める
			
		}else{
			initPos.x = 6.0f;
			targetPos.x = 4.0f;
		}
		
		//for debug
		//常に右下から出るように
		//initPos.z = -4.0f;
		//targetPos.z = -2.0f;
		//initPos.x = 6.0f;
		//targetPos.x = 4.0f;

		//GameObject enemy = (GameObject)GameObject.Instantiate(enemies[Random.Range(0, enemies.Length)], initPos, Quaternion.identity);
		GameObject enemy = (GameObject)GameObject.Instantiate(enemies[Random.Range(0, enemies.Length)], initPos, Quaternion.identity);
		enemy.GetComponent<Enemy>().wave = this;
		enemy.SendMessage("Show", targetPos);
		//iTween.MoveTo(enemy, targetPos, 1);
		*/
	}

	public void EnemyDestroyed(){
		destroyed++;
		if(destroyed >= num){
			//Debug.Log("wave finish");
			GameObject.FindWithTag("GameController").SendMessage("WaveClear");
		}
	}
}
