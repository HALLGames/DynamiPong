using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class PowerupPongPowerupManager : NetworkedBehaviour
{
    public enum PowerupType { None, BallSpeedDown, BallSpeedUp, PaddleSpeedDown, PaddleSpeedUp, BallReverse, 
        WorldReverse, PaddleExpand, PaddleShrink};

    public Transform[] powerupSpawnpoints;

    public Sprite ballSpeedDownSprite;
    public Sprite ballSpeedUpSprite;
    public Sprite paddleSpeedDownSprite;
    public Sprite paddleSpeedUpSprite;
    public Sprite ballReverseSprite;
    public Sprite worldReverseSprite;
    public Sprite paddleExpandSprite;
    public Sprite paddleShrinkSprite;

    private Transform powerupObjects;
    private PowerupPongPowerup powerupPrefab;
    [HideInInspector]
    public List<PowerupPongPowerup> spawnedPowerups;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void NetworkStart()
    {
        powerupPrefab = Network.GetPrefab<PowerupPongPowerup>("PowerupPongPowerup");
        powerupObjects = new GameObject("Powerups").transform;
        spawnedPowerups = new List<PowerupPongPowerup>();


        if (IsServer)
        {
            // Repeated spawning powerups
            InvokeRepeating("spawnPowerup", 2, 8);
            InvokeRepeating("spawnPowerup", 6, 8);
        }
    }

    private void spawnPowerup()
    {
        // Choose random spawnpoint location
        Transform spawnpoint;
        int spawnIndex = new System.Random().Next(0, powerupSpawnpoints.Length - 1);
        spawnpoint = powerupSpawnpoints[spawnIndex];

        // Spawn
        PowerupPongPowerup powerup = Instantiate(powerupPrefab, spawnpoint.position, Quaternion.identity, powerupObjects);
        powerup.GetComponent<NetworkedObject>().Spawn();
        spawnedPowerups.Add(powerup);
    }

    public Sprite getSprite(PowerupType power)
    {
        switch (power)
        {
            case PowerupType.BallSpeedDown:
                return ballSpeedDownSprite;
            case PowerupType.BallSpeedUp:
                return ballSpeedUpSprite;
            case PowerupType.PaddleSpeedDown:
                return paddleSpeedDownSprite;
            case PowerupType.PaddleSpeedUp:
                return paddleSpeedUpSprite;
            case PowerupType.BallReverse:
                return ballReverseSprite;
            case PowerupType.WorldReverse:
                return worldReverseSprite;
            case PowerupType.PaddleExpand:
                return paddleExpandSprite;
            case PowerupType.PaddleShrink:
                return paddleShrinkSprite;
        }
        return null;
    }

    public float getModifier(PowerupType power)
    {
        switch (power)
        {
            case PowerupType.BallSpeedDown:
                return 0.75f;
            case PowerupType.BallSpeedUp:
                return 1.5f;
            case PowerupType.PaddleSpeedDown:
                return 0.5f;
            case PowerupType.PaddleSpeedUp:
                return 2f;
            case PowerupType.BallReverse:
                return -1f;
            case PowerupType.PaddleExpand:
                return 2f;
            case PowerupType.PaddleShrink:
                return 0.5f;
        }
        return 0;
    }

    public int getDuration(PowerupType power)
    {
        switch (power)
        {
            case PowerupType.BallSpeedDown:
                return 10;
            case PowerupType.BallSpeedUp:
                return 10;
            case PowerupType.PaddleSpeedDown:
                return 10;
            case PowerupType.PaddleSpeedUp:
                return 10;
            case PowerupType.PaddleExpand:
                return 5;
            case PowerupType.PaddleShrink:
                return 5;
        }
        return 0;
    }
}
