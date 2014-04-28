using UnityEngine;
using System.Collections;

public class CameraResize : MonoBehaviour {

	public static float screenScale = 1f;

    //float w = Screen.width/1024f;
    //float h = Screen.height/768f;
	


	// Use this for initialization
	void Start () {

        //screenScale = (1024f/Screen.width);

        //transform.localScale += new Vector3(w, h, 0f);

        
		// This was originally here to account for retina ipads (2048x1536)
		// But it gets in the way of larger PC resolutions. 
		// Should actually just check directly for a retina ipad through iPhone.generation
		if (Screen.width >= 2048 && Screen.height >= 1536) {
			screenScale = 0.5f;
			camera.orthographicSize = (Screen.height/4f)/Constants.PTMRatio;
		}
		else if (Screen.width >= 960)
			camera.orthographicSize = (Screen.height/2f)/Constants.PTMRatio;
		else {
			screenScale = 2f;
			camera.orthographicSize = (Screen.height)/Constants.PTMRatio;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
