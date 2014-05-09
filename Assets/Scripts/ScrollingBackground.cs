﻿using UnityEngine;
using System.Collections;

public class ScrollingBackground : MonoBehaviour {
	private Vector3 backPos;
	public float width = 14.22f;
	public float height = 0f;
	private float X;
	private float Y;
	
	//Doesn't loop immediately and I have no idea why...
	void OnBecameInvisible()
	{
		//calculate current position
		backPos = gameObject.transform.position;
		//calculate new position
		print (backPos);
		X = backPos.x + width*2;
		Y = backPos.y + height*2;
		//move to new position when invisible
		gameObject.transform.position = new Vector3 (X, Y, 0f);
	}
	
}