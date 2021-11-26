//Emre Avan 22.11.2021
//
//Run State in the Car State Pattern
//This state is responsible for the user input and saving Car object transform values at Fixed Update.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run : State
{
    Transform mTransform;

    public Run(CarScript car) : base(car)
    {
        mTransform = Car.transform;
        Car.trailRenderer.enabled = true;
    }

    public override void Tick()
    {
        if (Input.GetKey("left")){
            mTransform.Rotate(0,0, Constants.mRotateSpeed * +1.0f, Space.World);
        }

        if (Input.GetKey("right")){
            mTransform.Rotate(0,0, Constants.mRotateSpeed * -1.0f, Space.World);
        }
        mTransform.position = mTransform.position + mTransform.up * Constants.mSpeed;        
    }

    public override void FixedTick()
    {
        //This solution may be update if user input is stored not car transform. 
        Car.mCarTransformList.Add(new CarScript.CarTransform(mTransform.position, mTransform.rotation));
    }

    //Level is finished
    public override void Finish()
    {
        Car.trailRenderer.Clear();
        Car.trailRenderer.enabled = false;
        LevelManager.instance.CarReachedToTarget();
        Car.SetState(new Replay(Car));
    }

    //Car is crashed in to a obstacle or a replaying car.
    public override void Die()
    {
        Car.trailRenderer.Clear();
        Car.trailRenderer.enabled = false;
        LevelManager.instance.CarCrashed();
        Car.mCarTransformList.Clear();
        Car.SetState(new Begin(Car));
    }
}
