﻿using UnityEngine;
using System.Collections;

public class bearAnim : MonoBehaviour {
    tk2dSpriteAnimator anim;
    bool walking;

	// Use this for initialization
	void Start () {
        anim = GetComponent<tk2dSpriteAnimator>();
	}

	// Update is called once per frame
	void Update () {
        if (Input.GetAxis("Horizontal") > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            anim.Play("walk");
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            transform.localScale = new Vector3(-1,1,1);
            anim.Play("walk");
        }
        else
        {
            anim.Play("idle");
        }
    }
}
