using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkHelpButton : MonoBehaviour
{
    public GameObject helpPanelPrefab;

    private GameObject helpPanel;

    private void Awake()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        helpPanel = Instantiate(helpPanelPrefab, canvas.transform);
        // helpPanel.transform.position = new Vector3(-740, -25);
        helpPanel.SetActive(false);
    }

    public void ToggleState()
    {
        // change the state from on to off and vice-versa
        helpPanel.SetActive(!helpPanel.activeSelf);
    }
}
