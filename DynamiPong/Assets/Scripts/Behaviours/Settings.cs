using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public GUISkin guiSkin;
    public Texture2D background, LOGO;
    public bool DragWindow = false;
    public string levelToLoadWhenClickedPlay = "";
    public string[] AboutTextLines = new string[0];


    private string clicked = "", MessageDisplayOnAbout = "About \n ";
    private Rect WindowRect = new Rect((Screen.width / 2) - 100, Screen.height / 2, 200, 200);
    private float volume = 1.0f;
    public void ToggleState()
    {
        GetComponent<Settings>().enabled = !GetComponent<Settings>().enabled; //this changes the state from on to off and vice-versa
    }

    private void OnGUI()
    {
        if (background != null)
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background);
        if (LOGO != null && clicked != "about")
            GUI.DrawTexture(new Rect((Screen.width / 2) - 100, 30, 200, 200), LOGO);

        GUI.skin = guiSkin;
        WindowRect = GUI.Window(1, WindowRect, optionsFunc, "Settings");

    }

    private void optionsFunc(int id)
    {

        GUILayout.Box("Volume");
        volume = GUILayout.HorizontalSlider(volume, 0.0f, 1.0f);
        AudioListener.volume = volume;
        if (GUILayout.Button("Back"))
        {
            ToggleState();
        }
        if (DragWindow)
            GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
    }


}

