using MLAPI.Transports.UNET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectionCanvas : MonoBehaviour
{
    // UI fields
    public InputField nameField;
    public InputField addressField;
    public InputField portField;
    public Toggle hostToggle;
    public Button connectButton;
    public Button backButton;
    public Text connectingText;
    public Text errorText;
    

    // Start is called before the first frame update
    void Start()
    {
        UnetTransport transport = FindObjectOfType<Network>().GetComponent<UnetTransport>();

        // UI Init
        addressField.text = transport.ConnectAddress;
        portField.text = transport.ConnectPort.ToString();
        hostToggle.isOn = false;
        errorText.text = "";
    }

    public void OnHostToggle()
    {
        addressField.interactable = false;
        portField.interactable = false;
    }

    /// <summary>
    /// Returns whether the port input field has a valid number inside
    /// </summary>
    /// <returns></returns>
    public bool hasValidPortInput()
    {
        try
        {
            Convert.ToInt32(portField.text);
            return true;
        } 
        catch (Exception)
        {
            errorText.text = "Port invalid. Please enter a valid port number.";
            return false;
        }
    }

    /// <summary>
    /// Disables most buttons from being interacted with during connection.
    /// </summary>
    public void disableUI()
    {
        nameField.interactable = false;
        addressField.interactable = false;
        portField.interactable = false;
        hostToggle.interactable = false;
        connectButton.interactable = false;
        connectingText.text = "Connecting...";
    }

    public void OnBackButton()
    {
        // Don't carry network into main menu
        Network network = FindObjectOfType<Network>();
        network.CancelConnection();
        Destroy(network.gameObject);

        SceneManager.LoadScene("MainMenu");
    }
}
