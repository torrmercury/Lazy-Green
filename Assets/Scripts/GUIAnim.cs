using UnityEngine;
using System.Collections;

public class GUIAnim : Stats
{
    tk2dSpriteAnimator GUIanim;
    int prevHealth = 2;
    int prevMood = 2;
    int prevHappy = 0;
    int prevMoney = 0;
    public string stat;
    public TextMesh text;
    string use;


    // Use this for initialization
    void Start()
    {
        GUIanim = GetComponent<tk2dSpriteAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stat == "Health")
        {
            healthChange();
        }
        else if (stat == "Mood")
        {
            moodChange();
        }
        else if (stat == "Money")
        {
            moneytext();
        }
        else if (stat == "Happy")
        {
            happytext();
        }
    }

    void healthChange()
    {
        if (prevHealth != health)
        {
            if (health == 0)
            {
                GUIanim.Play("Healthy");
            }
            else if (health == 1)
            {
                GUIanim.Play("NotHealthy");
            }
            else if (health == 2)
            {
                GUIanim.Play("Sickly");
            }
            prevHealth = health;
        }
    }

    void moodChange()
    {
        if (prevMood != mood)
        {
            if (mood == 0)
            {
                GUIanim.Play("Happy");
            }
            else if (mood == 1)
            {
                GUIanim.Play("Neutral");
            }
            else if (mood == 2)
            {
                GUIanim.Play("Sad");
            }
            prevMood = mood;
        }
    }

    void happytext()
    {
        if (prevHappy != happy)
        {
            text.text = happy.ToString(use);
            prevHappy = happy;
        }
    }

    void moneytext()
    {
        if (prevMoney != money)
        {
            text.text = money.ToString(use);
            prevMoney = money;
        }
    }
}
