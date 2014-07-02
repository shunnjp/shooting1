using UnityEngine;
using System.Collections;

public class TouchController : MonoBehaviour {

	public GameObject player;
	public int id;
	public TouchType type;
	
	public enum TouchType {
		Player,
		Aim
	};

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log(Input.touchCount);
		if(Input.touchCount > 0){
			foreach(Touch touch in Input.touches){
				if(id == touch.fingerId){
					switch(type){
						case TouchType.Player:
							switch(touch.phase){
								case TouchPhase.Began:
									//Debug.Log("touchBegan");
									break;
								case TouchPhase.Moved:
								case TouchPhase.Stationary:
									player.SendMessage("MoveTo", touch.position);
									break;
								case TouchPhase.Ended:
									//player.SendMessage("hideDragger", touch.position);
									player.SendMessage("RemoveTouchController", gameObject.GetComponent<TouchController>());
									GameObject.Destroy(gameObject);
									break;
								default:
									break;
							}						
							break;
						case TouchType.Aim:
							switch(touch.phase){
								case TouchPhase.Began:
									
									break;
								case TouchPhase.Moved:
								case TouchPhase.Stationary:
									//Aimを移動
									player.SendMessage("MoveAim", touch.position);
									break;
								case TouchPhase.Ended:
									player.SendMessage("RemoveTouchController", gameObject.GetComponent<TouchController>());
									GameObject.Destroy(gameObject);
									break;
								default:
									break;
							}
							break;
						default:
							break;
					}//switch
				}//if
			}//for
		}//if
	}
}
