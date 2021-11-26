//Emre Avan 22.11.2021
//
//Helper Class for changing states
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    protected State mState;

    public void SetState(State startingState)
    {
        mState = startingState;
        startingState.Start(); 
    }
}
