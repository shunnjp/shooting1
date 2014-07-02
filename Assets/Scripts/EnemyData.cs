using UnityEngine;
using System.Collections;

public struct EnemyData {

	public string name;
	public float initX;
	public float initY;
	public float targetX;
	public float targetY;
	public EnemyData(string n, float ix, float iy, float tx, float ty){
		name = n;
		initX = ix;
		initY = iy;
		targetX = tx;
		targetY = ty;
	}
}
