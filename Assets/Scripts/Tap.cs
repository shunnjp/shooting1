using UnityEngine;
using System.Collections;

public struct Tap {

	public Vector3 position;
	public TapType type;
	public int fingerId;//マウスクリックの場合は-1
	public enum TapType{
		Touch,
		Click
	}

	public Tap(Vector3 p, TapType t, int id){
		position = p;
		type = t;
		fingerId = id;
	}
}
