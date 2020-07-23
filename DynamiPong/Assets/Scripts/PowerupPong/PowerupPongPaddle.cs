using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class PowerupPongPaddle : PaddleBehaviour
{
    protected Color leftColor = Color.green;
    protected Color rightColor = Color.red;

    private PowerupPongGameManager gameManager;
    private PowerupPongPowerupManager powerupManager;
    protected PowerupPongPowerupManager.PowerupType power;
    protected bool onLeft;

    private PowerupPongCanvas levelCanvas;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        levelCanvas = FindObjectOfType<PowerupPongCanvas>();
        gameManager = FindObjectOfType<PowerupPongGameManager>();
        powerupManager = FindObjectOfType<PowerupPongPowerupManager>();

        power = PowerupPongPowerupManager.PowerupType.None;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (IsOwner)
        {
            // Use powerup on space bar down
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (power != PowerupPongPowerupManager.PowerupType.None)
                {
                    usePowerup();
                }
            }
        }
    }

    // Custom player movement
    protected override void playerMovement()
    {
        base.playerMovement();
    }

    // Custom bot movement
    protected override void botMovement()
    {
        base.botMovement();

        if (power != PowerupPongPowerupManager.PowerupType.None)
        {
            // Use powerup in 3 seconds
            Invoke("usePowerup",3);
        }
    }

    // Custom initialization
    public override void init(bool onLeft)
    {
        base.init(onLeft);

        Color color = onLeft ? leftColor : rightColor;
        GetComponent<SpriteRenderer>().color = color;
        InvokeClientRpcOnEveryone(UpdateColorOnClient, color);
        this.onLeft = onLeft;
    }

    public override void setupBot()
    {
        base.setupBot();

        onLeft = false;
        Color color = rightColor;
        GetComponent<SpriteRenderer>().color = color;
        InvokeClientRpcOnEveryone(UpdateColorOnClient, color);
    }

    [ClientRPC]
    public void UpdateColorOnClient(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    /// <summary>
    /// Gives the paddle the powerup
    /// </summary>
    /// <param name="powerup"></param>
    /// <returns>False if paddle already has a powerup. True if powerup was granted.</returns>
    public bool givePowerup(PowerupPongPowerup powerup)
    {
        if (powerup != null && power == PowerupPongPowerupManager.PowerupType.None)
        {
            power = powerup.power;
            if (isBot)
            {
                GivePowerupOnClient(power);
            }
            InvokeClientRpcOnEveryone(GivePowerupOnClient, power);
            return true;
        }
        return false;
    }

    [ClientRPC]
    public void GivePowerupOnClient(PowerupPongPowerupManager.PowerupType power)
    {
        if (IsOwner)
        {
            this.power = power;
        }
        levelCanvas.setPowerupImage(power, onLeft);
    }

    private void usePowerup()
    {
        switch (power)
        {
            case PowerupPongPowerupManager.PowerupType.BallSpeedDown:
                tellServerToChangeBallSpeed(power);
                break;
            case PowerupPongPowerupManager.PowerupType.BallSpeedUp:
                tellServerToChangeBallSpeed(power);
                break;
            case PowerupPongPowerupManager.PowerupType.BallReverse:
                if (isBot || IsHost)
                {
                    gameManager.ReverseBallOnServer();
                }
                else
                {
                    InvokeServerRpc(gameManager.ReverseBallOnServer);
                }
                break;
            case PowerupPongPowerupManager.PowerupType.PaddleSpeedDown:
                tellServerToChangePaddleSpeed(power);
                break;
            case PowerupPongPowerupManager.PowerupType.PaddleSpeedUp:
                StartCoroutine(ChangePaddleSpeed(powerupManager.getModifier(power), powerupManager.getDuration(power)));
                break;
            case PowerupPongPowerupManager.PowerupType.PaddleExpand:
                StartCoroutine(ChangePaddleScale(powerupManager.getModifier(power), powerupManager.getDuration(power)));
                break;
            case PowerupPongPowerupManager.PowerupType.PaddleShrink:
                tellServerToChangePaddleScale(power);
                break;
            case PowerupPongPowerupManager.PowerupType.WorldReverse:
                if (isBot || IsHost)
                {
                    gameManager.WorldReverse();
                }
                else
                {
                    InvokeServerRpc(gameManager.WorldReverse);
                }
                break;
        }

        levelCanvas.clearPowerupImage(onLeft);
        power = PowerupPongPowerupManager.PowerupType.None;
    }

    private void tellServerToChangeBallSpeed(PowerupPongPowerupManager.PowerupType power)
    {
        if (isBot || IsHost)
        {
            gameManager.ChangeBallSpeedOnServer(
                powerupManager.getModifier(power),
                powerupManager.getDuration(power));
        }
        else
        {
            InvokeServerRpc(
                gameManager.ChangeBallSpeedOnServer,
                powerupManager.getModifier(power),
                powerupManager.getDuration(power));
        }
    }

    private void tellServerToChangePaddleSpeed(PowerupPongPowerupManager.PowerupType power)
    {
        if (isBot || IsHost)
        {
            gameManager.ChangeRemotePaddleSpeedOnClient(
                powerupManager.getModifier(power),
                powerupManager.getDuration(power),
                onLeft);
        }
        else
        {
            InvokeServerRpc(
                gameManager.ChangeRemotePaddleSpeed,
                powerupManager.getModifier(power),
                powerupManager.getDuration(power),
                onLeft);
        }
    }

    private void tellServerToChangePaddleScale(PowerupPongPowerupManager.PowerupType power)
    {
        if (isBot || IsHost)
        {
            gameManager.ChangeRemotePaddleScaleOnClient(
                powerupManager.getModifier(power),
                powerupManager.getDuration(power),
                onLeft);
        }
        else
        {
            InvokeServerRpc(
                gameManager.ChangeRemotePaddleScale,
                powerupManager.getModifier(power),
                powerupManager.getDuration(power),
                onLeft);
        }
    }

    [ClientRPC]
    public void ChangePaddleSpeedOnClient(float modifier, int duration)
    {
        if (IsOwner)
        {
            ChangePaddleSpeed(modifier, duration);
        }
    }

    public IEnumerator ChangePaddleSpeed(float modifier, int duration)
    {
        float oldSpeed = speed;
        speed *= modifier;
        yield return new WaitForSeconds(duration);
        // If duration < 0, effect lasts forever
        if (duration >= 0)
        {
            
            speed = oldSpeed;
        }
        yield break;
    }

    [ClientRPC]
    public void ChangePaddleScaleOnClient(float modifier, int duration)
    {
        if (IsOwner)
        {
            ChangePaddleScale(modifier, duration);
        }
    }

    public IEnumerator ChangePaddleScale(float modifier, int duration)
    {
        Vector3 oldScale = transform.localScale;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * modifier, transform.localScale.z);
        yield return new WaitForSeconds(duration);
        // If duration < 0, effect lasts forever
        if (duration >= 0)
        {
            transform.localScale = oldScale;
        }
        yield break;
    }
}

