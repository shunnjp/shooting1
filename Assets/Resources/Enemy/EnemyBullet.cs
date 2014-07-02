using UnityEngine;
using System.Collections;

public class EnemyBullet : MonoBehaviour {

	float speed = 2.0f;

	// Use this for initialization
	void Start () {
		GameObject player = GameObject.FindWithTag("Player");
		transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position);
		rigidbody.AddForce(transform.forward * speed, ForceMode.VelocityChange);
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.position.x < -5.0f || transform.position.x > 5.0f || transform.position.z < -5.0f || transform.position.z > 5.0f){
			//Debug.Log("Destroy : " + transform.position);
			GameObject.Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "Player"){
			GameObject.Destroy(gameObject);
			other.gameObject.SendMessage("ApplyDamage", 1);
		}
	}
}
