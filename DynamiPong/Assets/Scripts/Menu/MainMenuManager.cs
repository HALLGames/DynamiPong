using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public AudioSource menuMusic;

    private MainMenuCanvas mainMenuCanvas;

    private void Awake()
    {
        if(Application.isBatchMode ||  Network.GetArgumentIndex("-server") != -1)
        {
            // Disable sound
            AudioListener.volume = 0f;

            // Load connection
            SceneManager.LoadScene("Connection");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mainMenuCanvas = FindObjectOfType<MainMenuCanvas>();
        mainMenuCanvas.creditsPanel.gameObject.SetActive(false);

        // If there is no background music, create it
        GameObject menuMusicObject = GameObject.FindGameObjectWithTag("MenuMusic");
        if (menuMusicObject == null)
        {
            menuMusicObject = Instantiate(menuMusic).gameObject;
            DontDestroyOnLoad(menuMusicObject);
        }
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
