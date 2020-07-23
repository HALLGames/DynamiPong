using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System.Dynamic;

public class PowerupPongPowerup : NetworkedBehaviour
{
    [HideInInspector]
    public PowerupPongPowerupManager.PowerupType power;
    [HideInInspector]
    public int duration;
    [HideInInspector]
    public float modifier;

    private PowerupPongPowerupManager powerupManager;
    private new SpriteRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 8);
        powerupManager = FindObjectOfType<PowerupPongPowerupManager>();
        renderer = GetComponent<SpriteRenderer>();

        setRandomPower();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void setRandomPower()
    {
        // TODO: Better weighted random system
        float rand = Random.value;
        if (rand < 0.001)
        {
            // 0.1%
            setPower(PowerupPongPowerupManager.PowerupType.WorldReverse);
        }
        else if (rand < 0.05)
        {
            // 4.9%
            setPower(PowerupPongPowerupManager.PowerupType.PaddleShrink);
        }
        else if (rand < 0.1)
        {
            // 5%
            setPower(PowerupPongPowerupManager.PowerupType.PaddleExpand);
        }
        else if (rand < 0.2)
        {
            // 10% 
            setPower(PowerupPongPowerupManager.PowerupType.BallReverse);
        }
        else if (rand < 0.4)
        {
            // 20%
            setPower(PowerupPongPowerupManager.PowerupType.BallSpeedDown);
        }
        else if (rand < 0.6)
        {
            // 20% 
            setPower(PowerupPongPowerupManager.PowerupType.BallSpeedUp);
        }
        else if (rand < 0.8)
        {
            // 20%
            setPower(PowerupPongPowerupManager.PowerupType.PaddleSpeedDown);
        }
        else if (rand <= 1)
        {
            // 20%
            setPower(PowerupPongPowerupManager.PowerupType.PaddleSpeedUp);
        }
    }

    private void setPower(PowerupPongPowerupManager.PowerupType power)
    {
        if (this.power == PowerupPongPowerupManager.PowerupType.None)
        {
            Destroy(gameObject);
        }

        this.power = power;
        modifier = powerupManager.getModifier(power);
        duration = powerupManager.getDuration(power);
        renderer.sprite = powerupManager.getSprite(power);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ball")
        {
            if (IsServer)
            {
                PowerupPongPaddle lastTouchedPaddle = collision.GetComponent<PowerupPongBall>().lastTouchedPaddle;
                if (lastTouchedPaddle != null)
                {
                    if (lastTouchedPaddle.givePowerup(this))
                    {
                        // If powerup was given to paddle, deactivate powerup object
                        // GetComponent<NetworkedObject>().UnSpawn();
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
