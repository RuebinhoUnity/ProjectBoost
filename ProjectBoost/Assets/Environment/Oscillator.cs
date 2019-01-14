using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {

    [SerializeField] Vector3 movementVector;

    //todo remove from inspector later
    [Range(0,1)] [SerializeField] float movementFactor;


    private Vector3 startingPosition;

	// Use this for initialization
	void Start () {

        startingPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPosition + offset;
	}
}
