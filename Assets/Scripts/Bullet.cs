using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float speed = 10.0f;

	// Use this for initialization
	void Start () {
		rigidbody.AddForce(transform.forward * speed, ForceMode.VelocityChange);
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.position.x < -5.0f || transform.position.x > 5.0f || transform.position.z < -5.0f || transform.position.z > 5.0f){
			//Debug.Log("Destroy : " + transform.position);
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "Enemy"){
			Destroy(gameObject);
			other.gameObject.SendMessage("ApplyDamage", 1);
		}
	}
}
