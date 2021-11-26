//Emre Avan 22.11.2021
//
//Replay State in the Car State Pattern
//At this time position and rotation values are stored in the list at Run State
//Stored values is set to the Car object at Fixed Update tick.
//This solution may be update if user input is stored not car transform. 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Replay : State
{
    int mPos = 0;
    public Replay(CarScript car) : base(car)
    {
        car.GetComponent<SpriteRenderer>().material = Car.OldMaterial;

        Car.trailRenderer.enabled = false;
    }

    public override void Restart()
    {
        mPos = 0;
    }

    
    public override void FixedTick()
    {
        if(mPos < Car.mCarTransformList.Count){
            Car.transform.position = Car.mCarTransformList[mPos].position;
            Car.transform.rotation = Car.mCarTransformList[mPos].rotation;
            mPos++;
        }
    }
}
