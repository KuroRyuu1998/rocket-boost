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

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }


    void Update()
    {
        if (state == State.Alive)
        {
            RespondThrustInput();
            RespondRotateInput();
        }
        

    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; }
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
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(dead);
        mainEngineParticles.Stop();
        deadParticles.Play();
        Invoke("Restart", LevelLoadDelay); //parameterise time                
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextScene", LevelLoadDelay); //parameterise time
    }

    private void Restart()
    {
        if(deadParticles.isPlaying)
        {
            deadParticles.Stop();
        }        
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }

    private void RespondRotateInput()
    {
        rigidBody.freezeRotation = true; // take manual control of rotation

        float rotationSpeed = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationSpeed);
        }
        //Make rigidBody constraints all value below.
        rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
    }
    private void RespondThrustInput()
    {        
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyForce();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyForce()
    {
        float thrustSpeed = mainThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * thrustSpeed);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        if(!mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
        }
        
    }
}
