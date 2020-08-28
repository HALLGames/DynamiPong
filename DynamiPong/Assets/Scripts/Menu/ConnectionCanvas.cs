using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MLAPI;
using System;

public class ConnectionCanvas : MonoBehaviour
{
    // UI fields
    public InputField nameField;
    public InputField addressField;
    public InputField portField;
    public Dropdown addressDropdown;
    public Dropdown portDropdown;
    public Toggle hostToggle;
    public Button connectButton;
    public Text connectingText;
    public Text errorText;

    private Network network;

    // Start is called before the first frame update
    void Start()
    {
        network = FindObjectOfType<Network>();

        // UI Init
        hostToggle.isOn = false;
        errorText.text = "";
        portField.text = network.defaultPort.ToString();
        initAddressDropdown();
        connectButton.onClick.AddListener(OnConnectClick);
    }

    private void initAddressDropdown()
    {
        string[] enumNames = Enum.GetNames(typeof(Network.ConnectionType));
        List<string> connectionTypeList = new List<string>(enumNames);
        addressDropdown.ClearOptions();
        addressDropdown.AddOptions(connectionTypeList);

        OnAddressDropdownChanged();
        addressDropdown.onValueChanged.AddListener((int i) => OnAddressDropdownChanged());
        portDropdown.onValueChanged.AddListener((int i) => OnPortDropdownChanged());
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

        // Add name to Connection Data
        byte[] connectionData = System.Text.Encoding.Default.GetBytes(nameField.text);
        NetworkingManager.Singleton.NetworkConfig.ConnectionData = connectionData;

        if (hostToggle.isOn)
        {
            network.connect(addressField.text, int.Parse(portField.text), Network.NetworkType.Host);
        } 
        else
        {
            network.connect(addressField.text, int.Parse(portField.text), Network.NetworkType.Client);
        }   

        // Timeout after 20 seconds
        Invoke("timeoutConnection", 20);
    }

    public void OnCancelClick()
    {
        connectButton.onClick.RemoveAllListeners();
        connectButton.onClick.AddListener(OnConnectClick);
        toggleConnecting(false);
        network.CancelConnection();
        CancelInvoke();
    }

    public void OnAddressDropdownChanged()
    {
        // Default values (Custom)
        addressField.text = "";
        addressField.interactable = true;
        portDropdown.gameObject.SetActive(false);
        portField.text = network.defaultPort.ToString();
        portField.interactable = true;
        hostToggle.interactable = true;

        // Change UI based on selected connection
        switch (addressDropdown.value)
        {
            case (int)Network.ConnectionType.Server:
                addressField.text = network.serverAddress;
                addressField.interactable = false;
                portDropdown.gameObject.SetActive(true);
                portField.interactable = false;
                OnPortDropdownChanged();
                hostToggle.interactable = false;
                break;
            case (int)Network.ConnectionType.LocalNetwork:
                addressField.text = network.localNetworkAddress;
                addressField.interactable = false;
                break;
            case (int)Network.ConnectionType.Localhost:
                addressField.text = network.localhostAddress;
                addressField.interactable = false;
                break;
            case (int)Network.ConnectionType.Custom:
                break;
        }
    }

    public void OnPortDropdownChanged()
    {
        switch (portDropdown.value)
        {
            case 0: // Server
                portField.text = "7777";
                break;
            case 1: // Local Network
                portField.text = "7778";
                break;
            case 2: // Localhost
                portField.text = "7779";
                break;
            case 3: // Custom
                portField.text = "7780";
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
        portDropdown.interactable = !isConnecting;

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
            OnAddressDropdownChanged();
        }
    }

    private void timeoutConnection()
    {
        OnCancelClick();
        errorText.text = "Connection timed out";
    }
}
