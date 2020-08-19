using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    public GameObject settingsPanelPrefab;

    private GameObject settingsPanel;

    private void Awake()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        settingsPanel = Instantiate(settingsPanelPrefab, canvas.transform);
        settingsPanel.SetActive(false);
    }

    public void ToggleState()
    {
        // change the state from on to off and vice-versa
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
}

