//Emre Avan 22.11.2021
//
//Singleton pattern class for managing the level

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    //Singleton Class
    public static LevelManager instance;

    //Caching entrance target gameobjects
    public struct EntranceTargetObjStruct 
    {
        public GameObject mEntrance;
        public GameObject mTarget;
    }
    EntranceTargetObjStruct[] mEntranceTargetObjArr;

    [SerializeField]
    private GameObject CarGameObject;

    List<GameObject> mCarList;

    public Animator mTransition;

    //Old material is used for replaying target objects.
    public Material OldMaterial;

    void Start()
    {
        //Singleton
        if (instance != null) {
            Destroy(gameObject);
        }else{
            instance = this;
        }

        //FindGameObjectsWithTag returns gameobject without any sorting. Therefore every couple of
        //Entrance and Target Gameobject should be searched.
        GameObject[] mEntranceObjList = GameObject.FindGameObjectsWithTag(Constants.mEntranceTag);
        GameObject[] mTargetObjList = GameObject.FindGameObjectsWithTag(Constants.mTargetTag);
        
        //No entrance zone but to be safe check sizes of the arrays.
        if(mEntranceObjList.Length != mTargetObjList.Length){
            Debug.LogWarning("There is a problem with the level, Car entrance and target positions don't match!!");
        }

        int mCarCount = mEntranceObjList.Length;
        mEntranceTargetObjArr = new EntranceTargetObjStruct[mCarCount];

        for(int i = 0; i < mCarCount; i++){
            EntranceTargetObjStruct mNewObj;
            mNewObj.mEntrance = mEntranceObjList[i];
            mNewObj.mTarget = null;
            mEntranceObjList[i].SetActive(false);

            int id = mEntranceObjList[i].GetComponent<EntranceTargetScript>().ID;
            for(int k = 0; k < mCarCount; k++)
            {
                if(id  == mTargetObjList[k].GetComponent<EntranceTargetScript>().ID){
                    mNewObj.mTarget = mTargetObjList[k];
                    mTargetObjList[k].SetActive(false);
                    break;
                }
            }

            //No entrance zone but to be safe check couple is created.
            if(mNewObj.mTarget == null){
                Debug.LogWarning("There is a problem with entrance and target object!!");
            }  

            mEntranceTargetObjArr[mEntranceObjList[i].GetComponent<EntranceTargetScript>().ID] = mNewObj;
        }

        mCarList = new List<GameObject>();

        //Creating first car
        EnableCar(0);
    }

    //This function is called from CarScript after a collision detection with target gameobject.
    //If every car reached to its destination, next level is going to be loaded.
    public void CarReachedToTarget()
    {
        StopTime();

        if(mCarList.Count < mEntranceTargetObjArr.Length){     
            ResetCarPositions();       
            mEntranceTargetObjArr[mCarList.Count -1].mEntrance.GetComponent<SpriteRenderer>().material = OldMaterial;
            mEntranceTargetObjArr[mCarList.Count -1].mTarget.GetComponent<SpriteRenderer>().material = OldMaterial;
            mEntranceTargetObjArr[mCarList.Count -1].mTarget.GetComponent<BoxCollider2D>().enabled = false;
            EnableCar(mCarList.Count);
        }else{
            LoadNextLevel();
        }
    }

    //Creates car gameobject and added to the list
    void EnableCar(int carId)
    {
        GameObject mNewCar = GameObject.Instantiate(CarGameObject, 
                                                    mEntranceTargetObjArr[carId].mEntrance.transform.position,
                                                    mEntranceTargetObjArr[carId].mEntrance.transform.rotation);
        //No need to enable start position gameobject
        //mEntranceTargetObjArr[carId].mEntrance.SetActive(true);
        mEntranceTargetObjArr[carId].mTarget.SetActive(true);
        mCarList.Add(mNewCar);
    }

    //Car is crashed to a obstacle or a replaying car.
    public void CarCrashed()
    {
        StopTime();
        ResetCarPositions();
    }

    //Resets the car positions before game continues.
    void ResetCarPositions()
    {
        int count = mCarList.Count;
        for(int i = 0; i < count; i++){
            mCarList[i].GetComponent<CarScript>().ResetCarPosition();
        }
    }

    public void LoadNextLevel()
    {
        if(SceneManager.sceneCount < SceneManager.GetActiveScene().buildIndex)
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
        else
            StartCoroutine(LoadLevel(0));
    }

    //Fade in Fade out animation is added to level.
    private IEnumerator LoadLevel(int index)
    {
        mTransition.SetTrigger("Start");
        yield return new WaitForSecondsRealtime(1);
        
        SceneManager.LoadScene(index);              
    }

    public void StartTime()
    {
        Time.timeScale = 1.0f;
    }

    public void StopTime()
    {
        Time.timeScale = 0.0f;
    }
}
