using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float speed = 10.0f; 

	// Use this for initialization
	void Start () {
		//rigidbody.AddForce(transform.forward * speed, ForceMode.VelocityChange);
		//rigidbody2D.AddForce(new Vector2(0, speed));
	}

	public void AddForce(Vector2 v){
		//Debug.Log (v.x + " , " + v.y);
		rigidbody2D.AddForce(v * speed * 50.0f);
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.position.x < -5.0f || transform.position.x > 5.0f || transform.position.y < -5.0f || transform.position.y > 5.0f){
			//Debug.Log("Destroy : " + transform.position);
			Destroy(gameObject);
		}
	}
	/*
	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "Enemy"){
			Destroy(gameObject);
			other.gameObject.SendMessage("ApplyDamage", 1);
		}
	}
	*/

	void OnTriggerEnter2D(Collider2D collider){
		//Debug.Log ("衝突　" + collider.gameObject);
		if(collider.gameObject.tag == "Enemy"){
			Destroy(gameObject);
			collider.gameObject.SendMessage("ApplyDamage", 1);
		}
	}
}
