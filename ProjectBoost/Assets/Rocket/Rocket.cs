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

    bool isTransitioning = false;

    [SerializeField] float levelLoadDelay = 1f;

    private bool debugMode = false;
    private bool debugCollisionsTurnedOff = false;

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
        if (!isTransitioning)
        {
            ProcessInput();
        }
    }

    private void ProcessInput()
    {
        RespondToThrustInput();
        RespondToRotateInput();

        if(Debug.isDebugBuild) { 
        RespondToDebugMode();
        }
    }

    private void RespondToDebugModeInput()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            debugCollisionsTurnedOff = !debugCollisionsTurnedOff;
        }
    }

    private void RespondToDebugMode()
    {
        if (!debugMode)
        {
            debugMode = true;
        }
        else
        {
            if (Input.GetKey(KeyCode.Delete))
            {
                debugMode = false;
            }

            RespondToDebugModeInput();
        }
    }

    private void RespondToRotateInput()
    {
        if (Input.GetKey(KeyCode.R))
        {
            ResetRocketPosition();
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            print("LeftArrow pressed");
            RotateManually(Time.deltaTime * rcsThrust);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            print("RightArrow pressed");
            RotateManually(-Time.deltaTime * rcsThrust);
        }

    }

    private void RotateManually(float rotationThisFrame)
    {
        rocketRigidbody.freezeRotation = true; // take manual control of rotation
        transform.Rotate(Vector3.forward * rotationThisFrame);
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
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust()
    {
        rocketAudio.Stop();
        mainEnginePS.Stop();
    }

    private void ApplyThrust()
    {
        print("Space pressed");
        rocketRigidbody.AddRelativeForce(vectorUp * mainBoosterThrust * Time.deltaTime);

        if (!rocketAudio.isPlaying)
        {
            rocketAudio.PlayOneShot(mainEngineSound);
        }

        mainEnginePS.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //ignore collisions when dead
        if (isTransitioning)
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
        if (!debugCollisionsTurnedOff)
        {
            //Load Level1/Scene0
            isTransitioning = true;
            rocketAudio.Stop();
            rocketAudio.PlayOneShot(deathSound);
            deathPS.Play();
            Invoke("LoadFirstLevel", levelLoadDelay);
            //ResetRocketPosition();
        }
    }

    private void StartSuccessSequence()
    {
        isTransitioning = true;
        rocketAudio.Stop();
        rocketAudio.PlayOneShot(successSound);
        successPS.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void LoadNextLevel()
    {
        int nextSceneToLoad = GetNextSceneIndexToLoad();
        SceneManager.LoadScene(nextSceneToLoad);
        successPS.Stop();
    }

    private int GetNextSceneIndexToLoad()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        int maxScene = SceneManager.sceneCountInBuildSettings;

        if (maxScene-1 == sceneIndex)
        {
            return 0;
        }
        else
        {
            return sceneIndex + 1;
        }
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
        deathPS.Stop();
    }

    private void ResetRocketPosition()
    {
        transform.position = rocketInitialPosition;
        transform.rotation = rocketInitialRotation;
        rocketRigidbody.velocity = Vector3.zero;
    }
}
