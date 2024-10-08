using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private State currentState;

    public void SetState(State newState)
    {
        if (currentState != null)
        {
            StopAllCoroutines();  // Stop any coroutines running in the previous state
            currentState.OnExit();
        }

        currentState = newState;
        currentState.EnterState();  // Setup the new state
    }

    // This will allow the state machine to update its current state
    void Update()
    {
        if (currentState != null)
        {
            currentState.Update();  // Call the current state's update method
        }
    }

    // Method to start a Coroutine in the current state
    public void RunCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}
