//Emre Avan 22.11.2021
//
//Responsible for the Entrance and Target Gameobjects.
//At this moment it only stores the Id of the object and
//hides the textbox when it is needed.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EntranceTargetScript : MonoBehaviour
{
    public Text mInfoText;
    [SerializeField] private int mWhichCar ;

    public int ID
    {
        get => mWhichCar;

        set{
            mWhichCar = value; 
            mInfoText.text = mWhichCar.ToString();
        }
    }

    void Awake()
    {
        mWhichCar = int.Parse(mInfoText.text);
    }

    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
