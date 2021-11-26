//Emre Avan 22.11.2021
//
//Car Class is used for player and replaying car. State Machine is used.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarScript : StateMachine
{
    //Position and Rotation Values are stored for replay mode.
    public struct CarTransform
    {
        public Vector3 position;
        public Quaternion rotation;

        public CarTransform(Vector3 posVal, Quaternion rotVal)
        {
            position = posVal;
            rotation = rotVal;
        }
    }
    public List<CarTransform> mCarTransformList;

    //Start position and rotation are stored to restart the Car
    Vector3 mStartPosition;
    Quaternion mStartRotation;

    public TrailRenderer trailRenderer;

    //Old material is used for replaying cars.
    public Material OldMaterial;

    void Start()
    {
        mCarTransformList = new List<CarTransform>();

        mStartPosition = transform.position;
        mStartRotation = transform.rotation;

        SetState(new Begin(this));
    }

    void Update()
    {
        mState.Tick();
    }

    //Crash into Replaying Car or Obstacle
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag.CompareTo(Constants.mObstacleTag) == 0 ||
                other.gameObject.tag.CompareTo(Constants.mCarTag) == 0 ){
            mState.Die();
        }else if(other.gameObject.tag.CompareTo(Constants.mTargetTag) == 0 ){
            mState.Finish();
        }  
    }

    //For saving Car position and rotation
    void FixedUpdate()
    {
        mState.FixedTick();
    }

    //This function is called to restart the transform of the car
    public void ResetCarPosition()
    {
        transform.position = mStartPosition;
        transform.rotation = mStartRotation;
        mState.Restart();
    }
}
