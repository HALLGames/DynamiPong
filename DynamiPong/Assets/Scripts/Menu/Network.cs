using System;
using System.Linq;
using UnityEngine;
using MLAPI;
using System.Collections.Generic;
using MLAPI.SceneManagement;
using MLAPI.Transports.UNET;
using System.Net;

public class Network : MonoBehaviour
{
    public enum NetworkType { None, Client, Server, Host }
    public enum ConnectionType { Server, LocalNetwork, Localhost, Custom}

    public Dictionary<ulong, int> connectedPlayerScores;
    private Dictionary<ulong, string> connectedPlayerNames;

    public string localhostAddress { get => "127.0.0.1"; }
    public string localNetworkAddress { get => getLocalNetworkAddress(); }
    public string serverAddress { get => "192.168.42.17"; } 
    public int defaultPort { get => 7777; }

    public bool debugServer;

    // Start is called before the first frame update
    void Start()
    {
        connectedPlayerNames = new Dictionary<ulong, string>();
        connectedPlayerScores = new Dictionary<ulong, int>();

        if (Application.isBatchMode || GetArgumentIndex("-server") != -1 || (Application.isEditor && debugServer))
        {
            // Launch server if headless, given arg, or server mode flagged (editor only)
            connect(getServerAddress(), getPort(), NetworkType.Server);
        }
    }

    public void connect(string address, int port, NetworkType networkType)
    {
        // Change transport
        UnetTransport transport = GetComponent<UnetTransport>();
        transport.ConnectAddress = address;
        transport.ConnectPort = port;

        startConnection(networkType);

        // Enter Lobby
        if (IsServer())
        {
            NetworkSceneManager.SwitchScene("Lobby");
        }
    }

    private void startConnection(NetworkType networkType)
    {
        switch(networkType)
        {
            case NetworkType.Client:
                startClient();
                break;
            case NetworkType.Server:
                startServer();
                break;
            case NetworkType.Host:
                startHost();
                break;
        }
    }

    public void startClient()
    {
        NetworkingManager.Singleton.StartClient();
    }

    public void startServer()
    {
        addCallbacks();
        NetworkingManager.Singleton.StartServer();
    }

    public void startHost()
    {
        addCallbacks();
        NetworkingManager.Singleton.StartHost();

        // Check Approval manually because host mode doesn't do it
        ApprovalCheck(NetworkingManager.Singleton.NetworkConfig.ConnectionData, NetworkingManager.Singleton.LocalClientId, null);
    }

    private void addCallbacks()
    {
        NetworkingManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkingManager.Singleton.OnClientDisconnectCallback += ClientDisconnectCallback;
    }

    // Gets approval and puts name into the list
    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkingManager.ConnectionApprovedDelegate callback)
    {
        // Approve if there are one or fewer players and if the client hasn't already connected
        bool approved = NetworkingManager.Singleton.ConnectedClientsList.Count <= 1 && !connectedPlayerNames.ContainsKey(clientId);

        if (approved)
        {
            string name = System.Text.Encoding.Default.GetString(connectionData);
            addConnectedPlayer(clientId, name);
        }

        callback?.Invoke(false, null, approved, null, null);
    }

    // Puts name in dictionary with clientId as the key
    public void addConnectedPlayer(ulong clientId, string name)
    {
        // If name is empty, call them "PlayerX", where X is the player count
        if (name == "")
        {
            name = "Player" + (connectedPlayerNames.Count + 1);
        }

        // If names are the same, add "(n)" after it, n increments.
        string newName = name;
        int i = 1;
        while (connectedPlayerNames.ContainsValue(newName))
        {
            newName = name + " (" + i + ")";
            i++;
        }

        // Add to dictionaries
        connectedPlayerNames.Add(clientId, newName);
        connectedPlayerScores.Add(clientId, 0);
    }

    private void ClientDisconnectCallback(ulong clientId)
    {
        if (connectedPlayerNames.ContainsKey(clientId))
        {
            connectedPlayerNames.Remove(clientId);
        }
    }

    /// <summary>
    /// Get the Dictionary of clientIds to Names
    /// </summary>
    /// <returns></returns>
    public Dictionary<ulong, string> getConnectedPlayerNames()
    {
        return connectedPlayerNames;
    }

    public void CancelConnection()
    {
        if (IsClient())
        {
            NetworkingManager.Singleton.StopClient();
        } 
        else if (IsServer())
        {
            NetworkingManager.Singleton.StopServer();
        } 
        else if (IsHost())
        {
            NetworkingManager.Singleton.StopHost();
        }
    }


    private string getLocalNetworkAddress()
    {
        if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
        {
            // Get local IPv4 address
            IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
            return entry.AddressList[entry.AddressList.Length - 1].ToString();
        }
        return localhostAddress;
    }

    private string getServerAddress()
    {
        string addressString = GetArgumentValue("-address");
        if (addressString != null)
        {
            if (addressString == "server")
            {
                return serverAddress;
            }
            else if (addressString == "localNetwork")
            {
                return localNetworkAddress;
            } 
            else if (addressString == "localhost")
            {
                return localhostAddress;
            }
            return addressString;
        }
        return serverAddress;
    }

    private int getPort()
    {
        string portString = GetArgumentValue("-port");
        int port = defaultPort;
        if (portString != null)
        {
            try
            {
                port = int.Parse(portString);
            } 
            catch(Exception)
            {
                
            }
        }
        return port;
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

    public static bool IsHost()
    {
        return NetworkingManager.Singleton.IsHost;
    }

    /// <summary>
    /// Return network type.
    /// </summary>
    /// <returns>Server, Client or Host. Returns None if type could not be determined.</returns>
    public static NetworkType GetNetworkType()
    {
        if (IsHost())
        {
            return NetworkType.Host;
        }
        if (IsServer())
        {
            return NetworkType.Server;
        }
        if (IsClient())
        {
            return NetworkType.Client;
        }

        return NetworkType.None;
    }


    /// <summary>
    /// Gets the value given for the given command line argument
    /// </summary>
    /// <param name="argument">The name of the argument</param>
    /// <returns>The value for the argument. Returns null if argument not found.</returns>
    /// Code from https://stackoverflow.com/questions/39843039/game-development-how-could-i-pass-command-line-arguments-to-a-unity-standalo
    public static string GetArgumentValue(string argument)
    {
        string[] args = Environment.GetCommandLineArgs();
        int argIndex = GetArgumentIndex(argument);
        if (argIndex >= 0 && args.Length > argIndex + 1)
        {
            return args[argIndex + 1];
        }
        return null;
    }

    /// <summary>
    /// Gets the index of the given command line argument
    /// </summary>
    /// <param name="argument">The name of the argument</param>
    /// <returns>The index of the argument. Returns -1 if not found.</returns>
    public static int GetArgumentIndex(string argument)
    {
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == argument)
            {
                return i;
            }
        }
        return -1;
    }
}
