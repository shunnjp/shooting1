using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public GameObject bulletPrefab;
	public GameObject touchControllerPrefab;
	public GameObject aim;
	public GameObject dragger;
	
	public float life = 3.0f;
	private bool isImmortal = true;
	
	public int shotInterval = 10;
	private int shotTimer = 0;

	private bool isFiring = false;
	
	public int telepoListeningTime = 20;
	
	//private Touch firstTouch;
	private Tap firstTouch;

	private Vector3 telepoScreenPoint;
	private int teleportationCountdown = 0;
	private bool isTeleportation = false;

	private bool isPlayerDraggingByMouse = false;

	private List<TouchController> touchControllers = new List<TouchController>();
	
	//ドラッグ中、前フレームでの指の位置
	private Vector3 previousMousePos;
	
	//最初のタップ後指を離さず押し続けているかどうか
	//押し続けている場合は一定時間経過後にAimを移動させるため使用
	private bool tapHold = false;
	// Use this for initialization

	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(isTeleportation) return;
		
		Fire ();
		
		if(tapHold && Input.GetMouseButtonUp(0)){
			tapHold = false;
		}
		
		//ドラッグ移動および瞬間移動
		foreach(Touch touch in Input.touches){
			if(touch.phase == TouchPhase.Began){
				CatchTap(new Tap(touch.position, Tap.TapType.Touch, touch.fingerId));
			}
		}

		//瞬間移動受付時間カウントダウン中
		if(teleportationCountdown > 0){
			//瞬間移動受付時間の減算
			teleportationCountdown -= 1;
			if(tapHold && telepoListeningTime - teleportationCountdown >= shotInterval){
				//tapHoldしたまま一定時間経過した場合はAimの位置を移動（逆にすぐに離した場合は瞬間移動受付時間が過ぎるまでAimを移動させない）;
				StartMoveAim(firstTouch);
				teleportationCountdown = 0;
				//tapHold = false;
			}else if(!tapHold && teleportationCountdown <= 0){
				//tapHoldしていない状態で瞬間移動せずに瞬間移動受付時間が終わった場合は最初のタップ位置にAimを移動させる
				//Debug.Log ("MoveAim");
				MoveAim(firstTouch.position);
				tapHold = false;
			}
		}


		//マウス操作用
		if(Input.GetMouseButtonDown(0)){
			//マウスクリックなので、Tapコンストラクタの第３引数は-1
			CatchTap(new Tap(Input.mousePosition, Tap.TapType.Touch, -1));
		}
		if(isPlayerDraggingByMouse){
			//isPlayerDraggingByMouseフラグが立っている間は自機をマウス位置に追従させる。
			MoveTo (Input.mousePosition);
			if (Input.GetMouseButtonUp (0)) {
				//マウスアップを検知したらプレイヤードラッグ停止
				isPlayerDraggingByMouse = false;
				HideDragger ();
			}
		}
	}

	public void Fire(){
		if(!isFiring) return;
		shotTimer--;
		//Debug.Log(shotInterval);
		if(shotTimer <= 0){
			shotTimer = shotInterval;
			GameObject b = (GameObject)Instantiate(bulletPrefab, transform.position, Quaternion.identity);
			//自機の位置とaimの位置から角度を算出し、ショットに適用する。
			float zAngle = GetAim (transform.position, aim.transform.position);
			//Debug.Log (b.transform.rotation.eulerAngles);
			//Debug.Log (zAngle);
			b.transform.Rotate (new Vector3(0.0f, 0.0f, zAngle));
			SetVelocityForRigidbody2D(zAngle, 10.0f, b);
		}
	}

	public float GetAim(Vector2 p1, Vector2 p2) {
		float dx = p2.x - p1.x;
		float dy = p2.y - p1.y;
		float rad = Mathf.Atan2(dy, dx);
		return rad * Mathf.Rad2Deg;
	}

	public void SetVelocityForRigidbody2D(float direction, float speed, GameObject t) {
		// Setting velocity.
		Vector2 v;
		v.x = Mathf.Cos (Mathf.Deg2Rad * direction) * speed;
		v.y = Mathf.Sin (Mathf.Deg2Rad * direction) * speed;
		t.rigidbody2D.velocity = v;
	}
	
	void CatchTap(Tap t){
		//Debug.Log ("GetMouseButtonDown");
		telepoScreenPoint = new Vector3(t.position.x, t.position.y, 5.0f);
		
		if(CheckStartPlayerDrag(t)) return;
		
		if(teleportationCountdown > 0 && Vector3.Distance(telepoScreenPoint, firstTouch.position) < Screen.height / 10){
			//最初のタップから一定カウント以内かつふたつのタップの距離が一定以内であれば瞬間移動尾成立
			//Debug.Log ("クリックによる瞬間移動");
			Teleportation(telepoScreenPoint);
		}else{
			//瞬間移動受付時間外にタップした場合はそれを最初のタップfirstTouchとし、瞬間移動受付時間開始
			teleportationCountdown = telepoListeningTime;
			firstTouch = t;
			//tapHold = true;//タップしてから押し続けた場合のフラグ
		}
	}

	//指定された位置へ移動
	void MoveTo(Vector2 position){
		Vector3 targetPos = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, 5.0f));
		gameObject.transform.Translate(targetPos - gameObject.transform.position, Space.World);
		RotateToAim();
	}

	//照準の方向へ向く
	void RotateToAim(){
		//transform.rotation = Quaternion.LookRotation(aim.transform.position - transform.position);
		transform.LookAt (aim.transform, Vector3.back);
	}

	void StartMoveAim(Tap t){
		//Debug.Log("startMoveAim");
		MoveAim((Vector3)t.position + new Vector3(0, 5, 0));
		if(t.fingerId != -1){
			//fingerIdが-1で無い場合（＝タッチの場合）のみTouchControllerをつくる
			AddTouchController(gameObject, t.fingerId, TouchController.TouchType.Aim);
			/*
			GameObject touchController = (GameObject)GameObject.Instantiate(touchControllerPrefab, transform.position, Quaternion.identity);
			TouchController tc = touchController.GetComponent<TouchController>();
			tc.player = gameObject;
			tc.id = t.fingerId;
			tc.type = TouchController.TouchType.Aim;
			touchControllers.Add(tc);
			*/
		}
	}

	//照準を移動する
	void MoveAim(Vector2 screenPos){
		if(!isFiring) isFiring = true;//ショット発射フラグが立っていなければ立てる
		aim.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 5.0f));
		RotateToAim();
	}

	//Playerドラッグ判定
	bool CheckStartPlayerDrag(Tap tap){
		//Debug.Log(Vector3(touch.position.x, touch.position.y, 5.0f));
		//タップ位置にPlayerDraggerのコライダがあるかどうかOverlapPointで調べる
		Vector2 touchPoint = (Vector2)Camera.main.ScreenToWorldPoint(new Vector3(tap.position.x, tap.position.y, 5.0f));
		if (dragger.GetComponent<Collider2D>().OverlapPoint(touchPoint)){
			Debug.Log("player drag : " + touchPoint);
			ShowDragger();

			if(tap.fingerId > -1){
				//fingetIdが-1出ない場合（＝タッチの場合）のみTouchControllerをつくる
				AddTouchController(gameObject, tap.fingerId, TouchController.TouchType.Player);
			}else{
				//マウスドラッグの場合
				isPlayerDraggingByMouse = true;
				//Updateにてプレイヤーをマウスに追従
			}
//			TouchController touchController = new TouchController(gameObject, tap.fingerId, TouchController.TouchType.Player);
			/*
			GameObject touchController = (GameObject)GameObject.Instantiate(touchControllerPrefab, transform.position, Quaternion.identity);
			TouchController tc = (TouchController)touchController.GetComponent<TouchController>();
			tc.player = gameObject;
			tc.id = tap.fingerId;
			tc.type = TouchController.TouchType.Player;
			touchControllers.Add(tc);
			*/
			//Debug.Log("playerdragです");
			return true;
		}else{
			//Debug.Log("playerdragじゃない");
			return false;
		}
	}
	/*
	bool CheckStartPlayerDrag(Touch touch){
		//Debug.Log(Vector3(touch.position.x, touch.position.y, 5.0f));
		//タップ位置にPlayerDraggerのコライダがあるかどうかレイキャストで調べる
		Vector3 rayOrigin = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 5.0f));
		rayOrigin.y = 5;
		int layerMask = 1 << 13;//PlayerDraggerレイヤーのみ対象とする
		if (Physics.Raycast (rayOrigin, Vector3.down, Mathf.Infinity, layerMask)){
			//Debug.Log("player drag : " + rayOrigin);
			ShowDragger();

			GameObject touchController = (GameObject)GameObject.Instantiate(touchControllerPrefab, transform.position, Quaternion.identity);
			TouchController tc = (TouchController)touchController.GetComponent<TouchController>();
			tc.player = gameObject;
			tc.id = touch.fingerId;
			tc.type = TouchController.TouchType.Player;
			touchControllers.Add(tc);
			//Debug.Log("playerdragです");
			return true;
		}else{
			//Debug.Log("playerdragじゃない");
			return false;
		}
	}
	*/

	void AddTouchController(GameObject player, int id, TouchController.TouchType type){
//		TouchController tc = new TouchController(player, id, type);
//		touchControllers.Add(tc);
		GameObject touchController = (GameObject)GameObject.Instantiate(touchControllerPrefab, Vector3.zero, Quaternion.identity);
		TouchController tc = (TouchController)touchController.GetComponent<TouchController>();
		tc.player = player;
		tc.id = id;
		tc.type = type;
		touchControllers.Add(tc);
	}

	void RemoveTouchController(TouchController tc){
		for(int i = 0; i<touchControllers.Count; i++){
			if(touchControllers[i] == tc){
				if(tc.type == TouchController.TouchType.Player){
					HideDragger();
				}
				touchControllers.RemoveAt(i);
				return;
			}
		}
	}

	void ShowDragger(){
		dragger.renderer.enabled = true;
		Animator anim = dragger.GetComponent<Animator>();
		//anim.enabled = true;
		anim.SetTrigger("Show");
	}

	void HideDragger(){
		dragger.renderer.enabled = false;
		Animator anim = dragger.GetComponent<Animator>();
		//anim.enabled = false;
	}

	//瞬間移動
	void Teleportation(Vector3 telepoScreenPoint){
		teleportationCountdown = 0;
		SetImmortal();
		isTeleportation = true;
		iTween.MoveTo(gameObject, iTween.Hash("position", Camera.main.ScreenToWorldPoint(telepoScreenPoint), "time", 0.2f, "oncomplete", "FinishTeleportation", "oncompletetarget", gameObject));
	}

	void FinishTeleportation(){
		UnsetImmortal();
		isTeleportation = false;
		RotateToAim();
	}


	void SetImmortal(){
		isImmortal = true;
	}

	void UnsetImmortal(){
		isImmortal = false;
	}

	//ダメージ
	void ApplyDamage(float damage){
		if(isImmortal) return;
		life -= damage;
		if(life <= 0.0f){
			//GameObject.Destroy(gameObject);
			GameObject.FindWithTag("GameController").SendMessage("GameOver");
			gameObject.GetComponent <Animator>().SetTrigger ("Die");
		}else{
			gameObject.GetComponent <Animator>().SetTrigger ("Damaged");
		}
	}
}
