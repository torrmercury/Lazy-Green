using UnityEngine;
using System.Collections;

public class Gui_Menu : Stats {

    public const string LINE_BREAK_WIN = "\r\n";
    public const string LINE_BREAK = "\n";
    string lineBreak;

	string mainText;
	string locationText;
    string button1Text;
    string button2Text;
    string button3Text;
    string button4Text;
    int localLocation;
    bool waterbottle = false;

	public bool gui_show = false;
	// Use this for initialization
    void Start()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer ||
                Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.WP8Player)
        {
            lineBreak = LINE_BREAK_WIN;
        }
    }
	
	// Update is called once per frame
	void Update () {
	}

	//turn GUI on when entering a specific area AND Vert axis is positive
	//NEEDS TRIGGERS!!!
    void OnTriggerStay(Collider location)
    {
        if (Input.GetAxis ("Vertical") > 0)
        {
            
            if (location.CompareTag("House") == true)
            {
                localLocation = 0;
                locationText = "House";
                mainText = ("You are at the house. In future builds, more text will be here"+lineBreak+"explaining the situation in which your character finds him/herself.");
                if (health > 0)
                {
                    button1Text = ("");
                }
                else
                {
                    button1Text = "Sleep";
                }
                button2Text = "Relax";
                button3Text = "Entertainment (TV/Computer/etc.)";
                button4Text = "Take out trash";
            }
                
            else if (location.CompareTag("Store") == true)
            {
                
                localLocation = 1;
                locationText = "Store";
                Debug.Log(locationText);
                mainText = "You are at the store. In future builds, more text will be here"+lineBreak+"explaining the situation in which your character finds him/herself.";
                if (waterbottle == false)
                {
                    button1Text = "Buy reusable water bottle";
                }
                else
                {
                    button1Text = ("");
                }
                button2Text = "Recycle plastics, glass, etc.";
                button3Text = "Entertainment (Movie ticket/Concert ticket/etc.)";
                if (health > 0)
                {
                    button4Text = "Medication";
                }
                else
                {
                    button4Text = ("");
                }
            }

            else if (location.CompareTag("Office") == true)
            {
                localLocation = 2;
                locationText = "Office";
                mainText = "You are at the office. In future builds, more text will be here"+lineBreak+"explaining the situation in which your character finds him/herself.";
                button1Text = "Productive Work";
                button2Text = "Semi-productive Work";
                button3Text = "Drink water from tank using paper cups";
                if (waterbottle == true)
                {
                    button4Text = "Drink water from tank using reusable bottle";
                }
                else
                {
                    button4Text = ("");
                }
            
            }
            gui_show = true;
		}

		if (Input.GetAxis ("Vertical") < 0) {
				gui_show = false;
		}
	}

	//Turn GUI off when exiting area
	//NEEDS TRIGGERS!!!
	void OnTriggerExit () 
    {
		gui_show = false;
	}

	void OnGUI() {

		if (gui_show == true) {
            
			// Container Box
			GUI.Box (new Rect (15, 30, 475, 600), locationText);

			// Making buttons
			if (GUI.Button (new Rect (25, 350, 225, 125), button1Text)) 
            {
				if (localLocation == 0)
                {
                    if (health > 0)
                    {
                        health += -1;
                    }
                    
                }

                if (localLocation == 1)
                {
                    if (money < 500)
                    {

                    }
                    else
                    {
                        money = -500;
                        waterbottle = true;
                    }
                }
                if (localLocation == 2)
                {
                    money += 150;
                    happy += -15;
                }
			}

			// Make the second button.
			if (GUI.Button (new Rect (25, 485, 225, 125), button2Text)) 
            {
                if (localLocation == 0)
                {
                    happy += 10;
                }
                if (localLocation == 1)
                {
                    happy += 5;
                    money += 10;
                }
                if (localLocation == 2)
                {
                    money += 75;
                    happy += -10;
                }
			}

			// Repeat
			if (GUI.Button (new Rect (255, 350, 225, 125), button3Text)) 
            {
                if (localLocation == 0)
                {
                    happy += 15;
                    money += -20;
                }
                if (localLocation == 1)
                {
                    happy += 30;
                    money += -35;
                }
                if (localLocation == 2)
                {
                    happy += 15;
                    health += 1;
                }
			}

			// Repeat
			if (GUI.Button (new Rect (255, 485, 225, 125), button4Text)) 
            {
                Debug.Log("Works");
                if (localLocation == 0)
                {
                    happy += 15;
                    health += 1;
                }
                if (localLocation == 1)
                {
                    money += -75;
                    health += 1;
                }
                if (localLocation == 2)
                {
                    happy += 5;
                }	
			}

			//Text Area
			GUI.Box (new Rect (25, 70, 460, 270), mainText);
		}
	}


}
