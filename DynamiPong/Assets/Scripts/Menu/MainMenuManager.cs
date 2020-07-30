using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public AudioSource menuMusic;

    private MainMenuCanvas mainMenuCanvas;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuCanvas = FindObjectOfType<MainMenuCanvas>();
        mainMenuCanvas.creditsPanel.gameObject.SetActive(false);

        // If there is no background music, create it
        GameObject menuMusicObject = GameObject.FindGameObjectWithTag("BackgroundMusic");
        if (menuMusicObject == null)
        {
            menuMusicObject = Instantiate(menuMusic).gameObject;
            DontDestroyOnLoad(menuMusicObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStartButton()
    {
        SceneManager.LoadScene("Connection");
    }

    public void OnExitButton()
    {
        Application.Quit();
    }

    public void OnCreditsButton()
    {
        // Activate panel, set to center of canvas
        mainMenuCanvas.creditsPanel.gameObject.SetActive(true);
        mainMenuCanvas.creditsPanel.transform.position = mainMenuCanvas.transform.position;
    }

    public void OnCreditsCloseButton()
    {
        mainMenuCanvas.creditsPanel.gameObject.SetActive(false);
    }
}
