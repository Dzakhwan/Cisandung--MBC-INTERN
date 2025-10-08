using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLogic : MonoBehaviour
{
    private GameObject mainMenu;
    private GameObject optionsMenu;
    private GameObject creditsMenu;
    private GameObject loading;

    public GameObject Audio;
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    void Start()
    {
        // Find references to canvas objects
        mainMenu = GameObject.Find("MainMenuCanvas");
        optionsMenu = GameObject.Find("OptionsCanvas");
        creditsMenu = GameObject.Find("CreditsCanvas");
        loading = GameObject.Find("LoadingCanvas");

        // Enable main menu canvas and disable others
        mainMenu.GetComponent<Canvas>().enabled = true;
        optionsMenu.GetComponent<Canvas>().enabled = false;
        creditsMenu.GetComponent<Canvas>().enabled = false;
        loading.GetComponent<Canvas>().enabled = false;

        // Disable controls, graphics, and audio game objects
        Audio.SetActive(false);
    }

    // Method to handle start button click
    public void StartButton()
    {
        // Enable loading canvas, disable main menu, play button sound, and load starting scene
        loading.GetComponent<Canvas>().enabled = true;
        mainMenu.GetComponent<Canvas>().enabled = false;
        audioManager.PlaySFX(audioManager.buttonClick);
        SceneManager.LoadScene("mainScene");
    }

    // Method to handle options button click
    public void OptionsButton()
    {
        // Play button sound, disable main menu, enable options menu, and show audio settings
        
        mainMenu.GetComponent<Canvas>().enabled = false;
        optionsMenu.GetComponent<Canvas>().enabled = true;
        audioManager.PlaySFX(audioManager.buttonClick);
        Audio.SetActive(true);
    }

   

    // Method to handle credits button click
    public void CreditsButton()
    {
        // Play button sound, disable main menu, and enable credits menu
        audioManager.PlaySFX(audioManager.buttonClick);
        mainMenu.GetComponent<Canvas>().enabled = false;
        creditsMenu.GetComponent<Canvas>().enabled = true;
    }

    // Method to handle exit game button click
    public void ExitGameButton()
    {
        // Play button sound, quit application, and log message
        audioManager.PlaySFX(audioManager.buttonClick);
        Application.Quit();
        Debug.Log("App Has Exited");
    }

    // Method to handle return to main menu button click
    public void ReturnToMainMenuButton()
    {
        // Play button sound, enable main menu, disable options and credits menu

        mainMenu.GetComponent<Canvas>().enabled = true;
        optionsMenu.GetComponent<Canvas>().enabled = false;
        creditsMenu.GetComponent<Canvas>().enabled = false;
        audioManager.PlaySFX(audioManager.buttonClick);
    }

    // Update is called once per frame
    void Update()
    {

    }
}