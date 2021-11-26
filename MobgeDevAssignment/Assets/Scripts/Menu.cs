//Emre Avan 25.11.2021
//
//This is responsible for the Main Menu and Pop Up Menu in the scene
//Created only for foundation. 
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    //Starts the game by loading first game scene
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    //Restarts the game by loading first game scene again
    public void RestartGame()
    {
        StartGame();
    }
}
