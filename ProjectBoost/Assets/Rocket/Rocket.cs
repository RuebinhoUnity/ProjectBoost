﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    Rigidbody rocketRigidbody;
    private Vector3 vectorUp;
    private Vector3 rocketInitialPosition;
    private Quaternion rocketInitialRotation;

    [SerializeField] private float rcsThrust = 100f;
    [SerializeField] private float mainBoosterThrust = 100f;

    AudioSource rocketAudio;
    [SerializeField] UnityEngine.Object rocketPrefab;

    // Use this for initialization
    void Start()
    {
        rocketRigidbody = GetComponent<Rigidbody>();
        vectorUp = Vector3.up;
        rocketAudio = GetComponent<AudioSource>();
        rocketInitialPosition = transform.position;
        rocketInitialRotation = transform.rotation;
        
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        Thrust();
        Rotate();
    }

    private void Rotate()
    {
        rocketRigidbody.freezeRotation = true; // take manual control of rotation
        float rotationThisFrame = Time.deltaTime * rcsThrust;

        if (Input.GetKey(KeyCode.R))
        {
            ResetRocketPosition();
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            print("LeftArrow pressed");

            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            print("RightArrow pressed");
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rocketRigidbody.freezeRotation = false; // rotation gets controlled by physics again
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            print("Space pressed");
            rocketRigidbody.AddRelativeForce(vectorUp * mainBoosterThrust);

            if (!rocketAudio.isPlaying)
            {
                rocketAudio.Play();
            }
        }
        else
        {
            rocketAudio.Stop();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing
                print("Collision is ok!");
                break;

            case "Fuel":
                // do nothing
                print("Fuel collected!");
                break;

            default:
                ResetRocketPosition();
                break;

        }
    }

    private void ResetRocketPosition()
    {
        transform.position = rocketInitialPosition;
        transform.rotation = rocketInitialRotation;
        rocketRigidbody.velocity = Vector3.zero;
    }
}
