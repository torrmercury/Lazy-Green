using UnityEngine;
using System.Collections;

public class ScrollingBackground : MonoBehaviour {
	private Vector3 backPos;
	public float width = 14.22f;
	private float X;
	
    void Start()
    {
    }

    void Update()
    {
        if (gameObject.transform.position.x < -width-.1f)
        {
            //calculate current position
            backPos = gameObject.transform.position;
            //calculate new position
            print(backPos);
            X = backPos.x + width * 2;
            //move to new position when invisible
            gameObject.transform.position = new Vector3(X, transform.position.y, transform.position.z);
        }
        else if (gameObject.transform.position.x > width+.1f)
        {
            //calculate current position
            backPos = gameObject.transform.position;
            //calculate new position
            print(backPos);
            X = backPos.x + width * -2;
            //move to new position when invisible
            gameObject.transform.position = new Vector3(X, transform.position.y, transform.position.z);
        }
    }
    /*
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
	*/
}