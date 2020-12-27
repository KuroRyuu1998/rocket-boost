using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector;

    [Range(0, 1)][SerializeField] float movementFactor; //0 for not move, 1 for move

    Vector3 startingPosition;

    [SerializeField] float period = 2f;

    void Start()
    {
        startingPosition = transform.position;
    }


    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (period <= Mathf.Epsilon) { return; }
        movementFactor = Mathf.PingPong(Time.time / period, 1); // Time.time/period to adjust the speed of the obstacle
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPosition + offset;
    }
}
