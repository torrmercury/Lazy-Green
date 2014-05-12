using UnityEngine;
using System.Collections;

public class Stats : MonoBehaviour {
    public static int mood = 0;
    public static int health = 0;
    public static int money = 5000;
    public static int happy = 100;
    public static int pollution = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (happy > 70)
        {
            mood = 0;
        }
        else if (happy > 40 && happy <= 70)
        {
            mood = 1;
        }
        else if (happy <= 40)
        {
            mood = 2;
        }


        
	}
}
