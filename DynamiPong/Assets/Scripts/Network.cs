using System;
using System.Linq;
using UnityEngine;
using MLAPI;

public class Network : MonoBehaviour
{
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
}
