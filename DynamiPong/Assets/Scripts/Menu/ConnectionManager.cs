using MLAPI.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public AudioSource menuMusic;

    // Start is called before the first frame update
    void Start()
    {
        // If there is no background music, create it
        GameObject menuMusicObject = GameObject.FindGameObjectWithTag("MenuMusic");
        if (menuMusicObject == null)
        {
            menuMusicObject = Instantiate(menuMusic).gameObject;
            DontDestroyOnLoad(menuMusicObject);
        }
    }
}
