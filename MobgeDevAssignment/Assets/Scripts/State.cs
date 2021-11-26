//Emre Avan 22.11.2021
//
//State definition
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{

    protected CarScript Car;

    public State(CarScript carVal)
    {
        Car = carVal;
    }

    public virtual void Start() { }
    public virtual void Tick() { }
    public virtual void FixedTick() { }
    public virtual void Die() { }
    public virtual void Finish() { }
    public virtual void Restart() { }
}
