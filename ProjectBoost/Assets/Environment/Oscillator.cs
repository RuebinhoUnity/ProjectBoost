﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {

    [SerializeField] Vector3 movementVector = new Vector3(0f,0f,0f);
    [SerializeField] float period = 2f;

    //todo remove from inspector later
    [Range(0,1)] [SerializeField] float movementFactor;


    private Vector3 startingPosition;

	// Use this for initialization
	void Start () {

        startingPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

        //set movementFactor by code

        if (period > 0f)
        {

            float cycles = Time.time / period;  //grows continually from 0
            const float tau = Mathf.PI * 2; // about 6.28
            float rawSinWave = Mathf.Sin(cycles * tau);  //goes from -1 to +1
            movementFactor = rawSinWave / 2f + 0.5f;
        }
        
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPosition + offset;
	}
}
