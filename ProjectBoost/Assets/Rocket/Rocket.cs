using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rocketRigidbody;
    private Vector3 vectorUp;
    private Vector3 rocketInitialPosition;
    private Quaternion rocketInitialRotation;

    [SerializeField] private float rcsThrust = 100f;
    [SerializeField] private float mainBoosterThrust = 100f;
    [SerializeField] private AudioClip mainEngineSound;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip deathSound;

    [SerializeField] private ParticleSystem mainEnginePS;
    [SerializeField] private ParticleSystem successPS;
    [SerializeField] private ParticleSystem deathPS;

    AudioSource rocketAudio;
    [SerializeField] UnityEngine.Object rocketPrefab;

    enum State { Alive, Dying, Transcending };
    private State rocketState = State.Alive;

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
        if (rocketState == State.Alive) { 
        ProcessInput();
        }
    }

    private void ProcessInput()
    {
        RespondToThrustInput();
        RespondToRotateInput();
    }

    private void RespondToRotateInput()
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

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            rocketAudio.Stop();
            mainEnginePS.Stop();
        }
    }

    private void ApplyThrust()
    {
        print("Space pressed");
        rocketRigidbody.AddRelativeForce(vectorUp * mainBoosterThrust);

        if (!rocketAudio.isPlaying)
        {
            rocketAudio.PlayOneShot(mainEngineSound);
        }

        mainEnginePS.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //ignore collisions when dead
        if (rocketState != State.Alive)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing
                print("Collision is ok!");
                break;

            case "Finish":
                // change to next level
                StartSuccessSequence();
                break;

            case "Fuel":
                // do nothing
                print("Fuel collected!");
                break;

            default:
                StartDeathSequence();
                break;

        }
    }

    private void StartDeathSequence()
    {
        //Load Level1/Scene0
        rocketState = State.Dying;
        rocketAudio.Stop();
        rocketAudio.PlayOneShot(deathSound);
        deathPS.Play();
        Invoke("LoadFirstLevel", 1f);
        //ResetRocketPosition();
    }

    private void StartSuccessSequence()
    {
        rocketState = State.Transcending;
        rocketAudio.Stop();
        rocketAudio.PlayOneShot(successSound);
        successPS.Play();
        Invoke("LoadNextLevel", 1f);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1);
        rocketState = State.Alive;
        successPS.Stop();
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
        rocketState = State.Alive;
        deathPS.Stop();
    }

    private void ResetRocketPosition()
    {
        rocketState = State.Alive;
        transform.position = rocketInitialPosition;
        transform.rotation = rocketInitialRotation;
        rocketRigidbody.velocity = Vector3.zero;
    }
}
