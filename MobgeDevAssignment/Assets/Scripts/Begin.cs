//Emre Avan 22.11.2021
//
//Begin State for the Car State Pattern
//Current car in the scene begins from this state and after
//a user input is occured, state is set to Run
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Begin : State
{
    public Begin(CarScript car) : base(car)
    {

    }

    //Update Function
    public override void Tick()
    {
        if (Input.anyKeyDown){
            LevelManager.instance.StartTime();
            Car.SetState(new Run(Car)); 
        }
    }
}
