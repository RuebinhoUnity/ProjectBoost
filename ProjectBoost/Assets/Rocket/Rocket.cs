using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

    Rigidbody rocketRigidbody;
    private Vector3 vectorUp;

    // Use this for initialization
    void Start () {
        rocketRigidbody = GetComponent<Rigidbody>();
        vectorUp = Vector3.up;

    }
	
	// Update is called once per frame
	void Update () {
        ProcessInput();
	}

    private void ProcessInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            print("Space pressed");
            rocketRigidbody.AddRelativeForce(vectorUp);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            print("LeftArrow pressed");
            transform.Rotate(Vector3.forward);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            print("RightArrow pressed");
            transform.Rotate(-Vector3.forward);
        }
    }
}
