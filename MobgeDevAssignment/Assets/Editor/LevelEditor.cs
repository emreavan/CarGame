//Emre Avan 21.11.2021
//
//Level Editor which can be loaded from the Windows
//Necessary scripts are added to the scene via Main Camera Object. Therefore it should 
//be chosen in the GUI and the scene must be initialized by clicking Initialize Scene
//Obstacles are loaded automatically in order to help designer but new Obstacles must be
//placed in the Resources/Obstacles folder.
//Important Note: At this moment there is a bug that when designer closes the Level Editor Window, 
//class loses its values. 

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor.SceneManagement;

public class LevelEditor : EditorWindow
{
    //Prefabs
    List<GameObject> ObstaclePrefabsList;
    private GameObject EntranceGameObject;
    private GameObject TargetGameObject;
    private GameObject CameraObject;
    private GameObject CarHolder;
    private GameObject ObstacleHolder;
    string[] ObstaclesNamesArray;

    //Class variables
    private int mObstacleCount = 0;
    private int mSelectedObstacle = 0;
    private int mNumberOfCars = 0;
    private GameObject mCameraObject;
    private GameObject mTransitionObj;
    List<LevelManager.EntranceTargetObjStruct> mEntranceTargetObjList;
    bool isSceneInitialized;

    public void Awake()
    {
        LoadObstacles();
        mEntranceTargetObjList = new List<LevelManager.EntranceTargetObjStruct>();
    }

    public LevelEditor()
    {
        isSceneInitialized = false;
    }

    [MenuItem("Window/Level Editor")]
    public static void ShowWindow( )
    {
        GetWindow<LevelEditor>("Level Editor");
        
    }

    void BeginVerticalBox(string name)
    {
        GUILayout.BeginVertical("box");
        EditorGUILayout.LabelField(name, EditorStyles.boldLabel);
    }

    void EndVerticalBox()
    {
        GUILayout.EndVertical();
    }

    void OnGUI()
    {
        minSize = new Vector2(300, 300);
        
        BeginVerticalBox("Camera");
        CameraObject = (GameObject) EditorGUILayout.ObjectField(CameraObject, typeof(GameObject), false); 
        EndVerticalBox();
        
        BeginVerticalBox("Start");
        if (GUILayout.Button("Initialize Scene")){
            InitializeScene();
        }
        EndVerticalBox();
        

        BeginVerticalBox("Obstacle");

        GUILayout.BeginHorizontal();
        GUILayout.Label(AssetPreview.GetAssetPreview(ObstaclePrefabsList[mSelectedObstacle]),
                                                        GUILayout.Width(70), GUILayout.Height(70)); 

        //GUILayout.FlexibleSpace();
        mSelectedObstacle = EditorGUILayout.Popup("Type", mSelectedObstacle, 
                                                        ObstaclesNamesArray, 
                                                        new GUIStyle( EditorStyles.popup ),
                                                        GUILayout.MinWidth(100));
        
        //GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EndVerticalBox();

        if (GUILayout.Button("Create Obstacle")){
            InstantiateObstacle();
        }

        BeginVerticalBox("Car");

        GUILayout.Label("Number of Cars " + mNumberOfCars.ToString());
        GUILayout.BeginHorizontal();
        GUILayout.Label("Car Entrance Obj");
        EntranceGameObject = (GameObject) EditorGUILayout.ObjectField(EntranceGameObject, typeof(GameObject), false);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Car Target Obj");
        TargetGameObject = (GameObject) EditorGUILayout.ObjectField(TargetGameObject, typeof(GameObject), false); 
        GUILayout.EndHorizontal(); 

        if(GUILayout.Button("Create Car Positions")) {
            CreateCarPositions();
        }
        
        EndVerticalBox();

        BeginVerticalBox("Save & Reset");
        
        if (GUILayout.Button("Save Scene")){
            SaveScene();  
        }     

        if (GUILayout.Button("Reset Scene")){
            ResetScene();  
        } 
        GUILayout.EndVertical();
    }

    //Creates a tuple of Entrance and Target Car Positions
    public void CreateCarPositions()
    {
        if(EntranceGameObject == null || TargetGameObject == null){
            EditorUtility.DisplayDialog("Empty GameObjects!",
                "Please Select Entrance and Target Objects First!", "OK");
            return;
        } 
        GameObject mNewEntranceObj = GameObject.Instantiate(EntranceGameObject, CarHolder.transform);
        mNewEntranceObj.name = "EntranceObj " + mEntranceTargetObjList.Count; 
        mNewEntranceObj.transform.position = Vector3.zero;
        mNewEntranceObj.GetComponent<EntranceTargetScript>().ID = mEntranceTargetObjList.Count;

        GameObject mNewTargetObj = GameObject.Instantiate(TargetGameObject, CarHolder.transform);
        mNewTargetObj.name = "TargetObj " + mEntranceTargetObjList.Count; 
        mNewTargetObj.transform.position = Vector3.zero + new Vector3(1,0,0);
        mNewTargetObj.GetComponent<EntranceTargetScript>().ID = mEntranceTargetObjList.Count;

        LevelManager.EntranceTargetObjStruct mNewObj;
        mNewObj.mTarget = mNewTargetObj;
        mNewObj.mEntrance = mNewEntranceObj;
        mEntranceTargetObjList.Add(mNewObj);
        mNumberOfCars = mEntranceTargetObjList.Count;
    }

    //Creates a gameobject from given obstacle type
    public void InstantiateObstacle()
    {
        GameObject mNewObstacle = GameObject.Instantiate(ObstaclePrefabsList[mSelectedObstacle], ObstacleHolder.transform);
        mNewObstacle.transform.position = Vector3.zero;
    }

    //This function needs to be called by clicking Initialize Scene in order to
    //play correctly. It creates necessary objects of the scene.
    public void InitializeScene()
    {
        ResetScene();
        mCameraObject = GameObject.Instantiate(CameraObject);
        mTransitionObj = mCameraObject.transform.GetChild(0).gameObject;
        mTransitionObj.SetActive(false);

        ObstacleHolder = new GameObject("Obstacle Holder");
        CarHolder = new GameObject("Car Entrance Target Holder");
        isSceneInitialized = true;
    }

    public void SaveScene()
    {
        if(!isSceneInitialized){
            EditorUtility.DisplayDialog("Not Initialized!",
                "Please Initialize the Scene First!", "OK");
            return;
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        mTransitionObj.SetActive(true);
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
            Debug.Log("Save Scene(s)");
        }
    }

    public void ResetScene()
    {
        var objects = GameObject.FindObjectsOfType(typeof(GameObject));
        foreach(GameObject obj in objects){

            if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(obj))
            {
                UnityEditor.PrefabUtility.UnpackPrefabInstance(obj,
                    UnityEditor.PrefabUnpackMode.Completely,
                    UnityEditor.InteractionMode.AutomatedAction);}
            else
            DestroyImmediate(obj);
        }

        GameObject mCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if(mCamera != null){
            DestroyImmediate(mCamera);
        }
    }

    // public void OnDestroy(){
    //     ObstaclePrefabsList.Clear();
    //     mEntranceTargetObjList.Clear();
    // }

    //Loads obstacles at the /Resources/Obstacles folder automatically.
    void LoadObstacles()
    {
        ObstaclePrefabsList = new List<GameObject>();
        string mFolder = Application.dataPath + "/Resources" + "/" + Constants.mObstacleDirectory;
        DirectoryInfo dir = new DirectoryInfo(mFolder);

        FileInfo[] mInfoList = dir.GetFiles("*.prefab",SearchOption.AllDirectories);
        foreach(FileInfo newInfo in mInfoList)
        {
            GameObject var = Resources.Load<GameObject>(Constants.mObstacleDirectory + "/" + 
                                                        Path.GetFileNameWithoutExtension(newInfo.Name));
            if(var.tag.CompareTo(Constants.mObstacleTag) == 0)
            {
                ObstaclePrefabsList.Add(var);
            }
        }
    
        mObstacleCount = ObstaclePrefabsList.Count;
        ObstaclesNamesArray = new string[mObstacleCount];
        for(int i = 0; i < mObstacleCount; i++)
        {
            ObstaclesNamesArray[i] = ObstaclePrefabsList[i].name;
        }
    }
}
