using MLAPI;
using MLAPI.Transports.UNET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectionCanvas : MonoBehaviour
{
    // UI fields
    public InputField nameField;
    public InputField addressField;
    public InputField portField;
    public Dropdown addressDropdown;
    public Toggle hostToggle;
    public Button connectButton;
    public Button backButton;
    public Text connectingText;
    public Text errorText;

    private Network network;
    private string localhostAddress = "127.0.0.1";
    private string localNetworkAddress;
    private string serverAddress;
    private string defaultPort = "7777";

    // Start is called before the first frame update
    void Start()
    {
        network = FindObjectOfType<Network>();
        updateAddresses();
        InvokeRepeating("updateAddresses", 15, 15);

        // UI Init
        hostToggle.isOn = false;
        errorText.text = "";
        portField.text = defaultPort;
        updateAddress();
        connectButton.onClick.AddListener(OnConnectClick);
        addressDropdown.onValueChanged.AddListener((int i) => updateAddress());
    }

    public void OnHostToggle()
    {
        addressField.interactable = false;
        portField.interactable = false;
    }

    public void OnBackButton()
    {
        // Don't carry network into main menu
        network.CancelConnection();
        Destroy(network.gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    public void OnConnectClick()
    {
        connectButton.onClick.RemoveAllListeners();
        connectButton.onClick.AddListener(OnCancelClick);
        toggleConnecting(true);
        network.connect();

        // Timeout 
        Invoke("timeoutConnection", 30);
    }

    public void OnCancelClick()
    {
        connectButton.onClick.RemoveAllListeners();
        connectButton.onClick.AddListener(OnConnectClick);
        toggleConnecting(false);
        network.CancelConnection();
    }

    public void updateAddress()
    {
        addressField.interactable = false;
        hostToggle.interactable = false;
        switch (addressDropdown.value)
        {
            case 0:
                addressField.text = serverAddress;
                break;
            case 1:
                addressField.text = localNetworkAddress;
                hostToggle.interactable = true;
                break;
            case 2:
                addressField.text = localhostAddress;
                hostToggle.interactable = true;
                break;
            case 3:
                addressField.text = "";
                addressField.interactable = true;
                hostToggle.interactable = false;
                break;
        }
    }

    /// <summary>
    /// Disables most buttons from being interacted with during connection.
    /// </summary>
    public void toggleConnecting(bool isConnecting)
    {
        nameField.interactable = !isConnecting;
        portField.interactable = !isConnecting;
        addressDropdown.interactable = !isConnecting;

        if (isConnecting)
        {
            connectingText.text = "Cancel";
            errorText.text = "Connecting...";
            hostToggle.interactable = false;
            addressField.interactable = false;
        } 
        else
        {
            connectingText.text = "Connect";
            errorText.text = "";
            updateAddress();
        }
    }

    private void updateAddresses()
    {
        if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
        {
            // Get local IPv4 address
            IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
            localNetworkAddress = entry.AddressList[entry.AddressList.Length - 1].ToString();

            // Get server address
            serverAddress = "192.168.42.17";
        }
        else
        {
            // No network connection
            localNetworkAddress = localhostAddress;
            serverAddress = localhostAddress;
            errorText.text = "No network connection available.";
        }
    }

    private void timeoutConnection()
    {
        OnCancelClick();
        errorText.text = "Connection timed out";
    }
}
