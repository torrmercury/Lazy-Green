using UnityEngine;
using System.Collections;

public class GUIAnim : Gui_Menu
{
    tk2dSpriteAnimator GUIanim;
    
    // Use this for initialization
    void Start()
    {
        GUIanim = GetComponent<tk2dSpriteAnimator>();
    }

    // Update is called once per frame
    void Update()
    {



        
    }

    void healthChange()
    {
        if (health == 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            GUIanim.Play("Healthy");
        }
        else if (health == 1)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            GUIanim.Play("NotHealthy");
        }
        else if (health == 2)
        {
            GUIanim.Play("Sickly");
        }
    }

    void moodChange()
    {
        if (mood == 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            GUIanim.Play("Happy");
        }
        else if (mood == 1)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            GUIanim.Play("Neutral");
        }
        else if (mood == 2)
        {
            GUIanim.Play("Sad");
        }
    }

}
