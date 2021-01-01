using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    [SerializeField] float rcsThrust = 250f;
    [SerializeField] float mainThrust = 750f;
    [SerializeField] float LevelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip dead;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deadParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    bool isEnableCollision = false;
    bool isTransitioning = false;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }


    void Update()
    {
        if (!isTransitioning)
        {
            RespondThrustInput();
            RespondRotateInput();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebug();
        }


    }

    private void RespondToDebug()
    {
       
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadScene(1);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (SceneManager.GetActiveScene().buildIndex > 0)
            {
                LoadScene(-1);
            }

        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            isEnableCollision = !isEnableCollision;
        }

    }

    private static void LoadScene(int order)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + order);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isTransitioning || isEnableCollision) { return; }
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }

    }

    private void StartDeathSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(dead);
        mainEngineParticles.Stop();
        deadParticles.Play();
        Invoke("Restart", LevelLoadDelay); //parameterise time                
    }

    private void StartSuccessSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextScene", LevelLoadDelay); //parameterise time
    }

    private void Restart()
    {
        if (deadParticles.isPlaying)
        {
            deadParticles.Stop();
        }
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void RespondRotateInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            RotateManually(rcsThrust * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RotateManually(-rcsThrust * Time.deltaTime);
        }
        //Make rigidBody constraints all value below.
        //rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
    }

    private void RotateManually(float rotationSpeed)
    {
        rigidBody.freezeRotation = true; // take manual control of rotation
        transform.Rotate(Vector3.forward * rotationSpeed);
        rigidBody.freezeRotation = false; // return manual control of rotation
    }

    private void RespondThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyForce();
        }
        else
        {
            StopApplyForce();
        }
    }

    private void StopApplyForce()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void ApplyForce()
    {
        float thrustSpeed = mainThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * thrustSpeed);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        if (!mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
        }

    }


}
