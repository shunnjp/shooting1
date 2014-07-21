using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public float life;
	public GameObject bulletPrefab;
	public GameObject explosionPrefab;
	public GameObject explosionPiecesPrefab;
	public Wave wave;
	
	public uint shotInterval = 100;
	private uint shotTimer = 0;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(life <= 0) return;
		//自機の方向を向く
		GameObject player = GameObject.FindWithTag("Player");
		if(!player) return;
		
		//transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position);
		
		shotTimer++;
		if(shotTimer >= shotInterval){
			//Debug.Log("enemy shot");
			shotTimer = 0;
			GameObject.Instantiate(bulletPrefab, transform.position, transform.rotation);
		}
	}

	void ApplyDamage(float damage){
		life -= damage;
		if(life <= 0){
			//死亡
			//コライダーを即座に消去
			Destroy(gameObject.collider2D);
			if(wave != null){
				//wave.SendMessage("EnemyDestroyed");
				wave.EnemyDestroyed();
			}
			//爆発を生成
			GameObject.Instantiate(explosionPrefab, transform.position, transform.rotation);
			GameObject.Instantiate(explosionPiecesPrefab, transform.position, transform.rotation);
			iTween.Stop(gameObject);//移動等のトゥイーンを停止
			//フェードアウト
			iTween.FadeTo(gameObject, iTween.Hash("alpha", 0.0f, "time", 0.5f, "oncomplete", "Die"));
		}else{
			//ダメージのカラー効果
			
			iTween.Stop(gameObject, "color");//すでにダメージ硬貨効果中の場合はStopする
			Color originalColor = gameObject.renderer.material.color;
			gameObject.renderer.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			iTween.ColorTo(gameObject, iTween.Hash ("color", originalColor, "time", 0.1f));
			
		}
	}

	void Show(Vector3 targetPos){
		iTween.MoveTo(gameObject, targetPos, 1);
	}

	//ApplyDamageでフェードアウトアニメーションを再生し、それが終了したら呼び出される。
	void Die(){
		//Debug.Log("enemy die");
		GameObject.Destroy(gameObject);
	}
}
