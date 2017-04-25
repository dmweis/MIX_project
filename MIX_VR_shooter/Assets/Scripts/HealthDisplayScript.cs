﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDisplayScript : MonoBehaviour {


	// Use this for initialization
	void Start () {
	}
	
    public void SetText(string text)
    {
        GetComponent<TextMesh>().text = text;
    }

	// Update is called once per frame
	void Update () {
        transform.LookAt(Camera.main.transform);
	}
}
