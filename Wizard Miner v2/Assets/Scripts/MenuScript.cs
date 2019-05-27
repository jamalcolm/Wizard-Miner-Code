using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
 * The MenuScript handles the menu button
 * functions.
*/

public class GameOptionsState
{
    public static float gameSpeed = 5;
    public static bool fixedCamera = false;
}

public class MenuScript : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject optionsMenu;
    public Slider gameSpeedSlider;
    public Toggle fixedCameraToggle;

	// Use this for initialization
	void Start () 
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
	}
	
    public void StartButtonOnClick()
    {
        //Load the main scene
        SceneManager.LoadScene("main");
    }

    public void OptionsOnClick()
    {
        //Go to options screen
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);

        gameSpeedSlider.value = GameOptionsState.gameSpeed;
        fixedCameraToggle.isOn = GameOptionsState.fixedCamera;
    }

    public void MainMenuOnClick()
    {
        //Go back to main menu
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);

        GameOptionsState.gameSpeed = gameSpeedSlider.value;
        GameOptionsState.fixedCamera = fixedCameraToggle.isOn;
    }

    public void ExitOnClick()
    {
        //Load the main scene
        Application.Quit();
    }
        
}
