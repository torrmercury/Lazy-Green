using UnityEngine;
using System.Collections;

public class BouncingBalloon : MonoBehaviour {

	public float amplitudeY;
	public float omegaY;

	private float index;

	public float ampMax;
	public float ampMin;

	public bool useRandom = false;
	
	void Update(){
		index += Time.deltaTime;
		if(!useRandom){
			float y = amplitudeY * Mathf.Sin(omegaY * index);
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + y, transform.localPosition.z);
		}
		else if(useRandom){
			float y2 = Random.Range(ampMin, ampMax) * Mathf.Sin(omegaY * index);
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + y2, transform.localPosition.z);
		}
	}
}
