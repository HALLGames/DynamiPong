using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public GUISkin guiSkin;

    private float width = 270;
    private float height = 480;
    private Settings window;
    private Rect WindowRect;

    private float volume = 1.0f;

    private void Awake()
    {
        window = GetComponent<Settings>();
        WindowRect = new Rect((Screen.width / 2) - (width / 2), (Screen.height / 2) - (height / 2), width, height);
    }

    public void ToggleState()
    {
        // change the state from on to off and vice-versa
        window.enabled = !window.enabled; 
    }

    private void OnGUI()
    {
        // GUI Setup
        GUI.skin = guiSkin;

        WindowRect = GUI.Window(1, WindowRect, optionsFunc, "Settings");
    }

    private void optionsFunc(int id)
    {
        // Space for Title
        GUILayout.Space(40);

        // Vertical Layout - Auto-fills space until bottom
        GUILayout.BeginVertical(GUILayout.ExpandHeight(true));

        // Volume Slider
        GUILayout.Box("Volume");
        volume = GUILayout.HorizontalSlider(volume, 0.0f, 1.0f);
        AudioListener.volume = volume;

        GUILayout.EndVertical();

        // Back Button
        if (GUILayout.Button("Back"))
        {
            ToggleState();
        }
    }


}

