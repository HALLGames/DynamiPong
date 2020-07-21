using System;
using System.Linq;
using UnityEngine;
using MLAPI;
using System.Collections.Generic;
using MLAPI.SceneManagement;
using UnityEngine.UI;

public class Network : MonoBehaviour
{
    public InputField nameField;
    public Toggle hostToggle;
    public Button connectButton;
    public Text connectingText;

    private Dictionary<ulong, string> connectedPlayerNames;


    // Start is called before the first frame update
    void Start()
    {
        connectingText.text = "";
        connectedPlayerNames = new Dictionary<ulong, string>();
    }

    public void OnConnectClick()
    {
        nameField.interactable = false;
        hostToggle.interactable = false;
        connectButton.interactable = false;
        connectingText.text = "Connecting...";

        if (Application.isEditor)
        {
            // Only data path to launch while in Editor
            string dataPathNormalized = Application.dataPath.ToUpperInvariant();

            // Start in server or client mode
            if (dataPathNormalized.Contains("CLIENT"))// Are we running as a client?
            {
                startClient();
            }
            else if (dataPathNormalized.Contains("SERVER")) // Are we running as a server?
            {
                startServer();
            }
            else // Else: Run as the host
            {
                startHost();
            }
        } 
        else // Launched from Binary
        {
            if (hostToggle.isOn)
            {
                startHost();
            } 
            else
            {
                startClient();
            }
        }

        // Enter Lobby
        if (IsServer())
        {
            NetworkSceneManager.SwitchScene("Lobby");
        }
    }

    private void startClient()
    {
        // Add name to Connection Data
        byte[] connectionData = System.Text.Encoding.Default.GetBytes(nameField.text);

        NetworkingManager.Singleton.NetworkConfig.ConnectionData = connectionData;
        NetworkingManager.Singleton.StartClient();
    }

    private void startServer()
    {
        NetworkingManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkingManager.Singleton.StartServer();
    }

    private void startHost()
    {
        NetworkingManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkingManager.Singleton.StartHost();

        // Add name because approval is not called for the host
        addConnectedPlayer(NetworkingManager.Singleton.LocalClientId, nameField.text);
    }

    // Gets approval and puts name into the list
    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkingManager.ConnectionApprovedDelegate callback)
    {
        // Approve if there are one or fewer players and if the client hasn't already connected
        bool approve = NetworkingManager.Singleton.ConnectedClientsList.Count <= 1 && !connectedPlayerNames.ContainsKey(clientId);

        if (approve)
        {
            string name = System.Text.Encoding.Default.GetString(connectionData);
            addConnectedPlayer(clientId, name);
        }

        callback(false, null, approve, null, null);
    }

    private void addConnectedPlayer(ulong clientId, string name)
    {
        // If name is empty, call them "PlayerX", where X is the player count
        if (name == "")
        {
            name = "Player" + connectedPlayerNames.Count.ToString();
        }

        // If names are the same, add "(n)" after it, n increments.
        string newName = name;
        int i = 0;
        while (connectedPlayerNames.ContainsValue(newName))
        {
            newName = name + " (" + i + ")";
            i++;
        }

        connectedPlayerNames.Add(clientId, name);
    }

    /// <summary>
    /// Get the Dictionary of clientIds to Names
    /// </summary>
    /// <returns></returns>
    public Dictionary<ulong, string> getConnectedPlayerNames()
    {
        return connectedPlayerNames;
    }

    //-------------------------------------------------------
    // Static Helper Functions
    //-------------------------------------------------------

    /// <summary>
    /// Return a prefab associated with the NetworkManager by name.
    /// </summary>
    /// <param name="name">Name of prefab</param>
    /// <returns>Prefab game object</returns>
    public static GameObject GetPrefab(string name)
    {
        return NetworkingManager.Singleton.NetworkConfig.NetworkedPrefabs
            .Select(p => p.Prefab)
            .Where(p => p.name == name)
            .FirstOrDefault();
    }

    /// <summary>
    /// Return a prefab associated with the NetworkManager by name.
    /// </summary>
    /// <param name="name">Name of prefab</param>
    /// <typeparam name="T">Type of prefab</typeparam>
    /// <returns>Prefab game object of type T</returns>
    public static T GetPrefab<T>(string name) where T : class
    {
        GameObject prefab = GetPrefab(name);
        return prefab == null ? (T)null : prefab.GetComponent<T>();
    }

    /// <summary>
    /// Flag indicating if networking is configured as server
    /// </summary>
    public static bool IsServer()
    {
        return NetworkingManager.Singleton.IsServer;
    }

    /// <summary>
    /// Flag indicating if networking is configured as server
    /// </summary>
    public static bool IsClient()
    {
        return NetworkingManager.Singleton.IsClient;
    }

    /// <summary>
    /// Return network type name.
    /// </summary>
    /// <returns>Server, Client or Host. Returns Unknown if type could not be determined.</returns>
    public static string NetworkType()
    {
        if (IsServer())
        {
            if (IsClient())
            {
                return "Host";
            }
            else
            {
                return "Server";
            }
        }

        if (IsClient())
        {
            return "Client";
        }

        return "Unknown";
    }

    
}
