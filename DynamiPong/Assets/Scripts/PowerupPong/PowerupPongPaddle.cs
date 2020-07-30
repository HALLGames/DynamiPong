using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.SceneManagement;

public class PowerupPongPaddle : PaddleBehaviour
{
    protected Color leftColor = Color.green;
    protected Color rightColor = Color.red;

    private PowerupPongGameManager gameManager;
    private PowerupPongPowerupManager powerupManager;

    protected PowerupPongPowerupManager.PowerupType power;
    protected bool onLeft;
    private float baseSpeed;
    private Vector3 baseScale;
    private AudioSource powerupSound;
    private AudioSource collectSound;

    private PowerupPongCanvas levelCanvas;

    public override void NetworkStart()
    {
        base.NetworkStart();
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        // Init local vars
        power = PowerupPongPowerupManager.PowerupType.None;
        baseSpeed = speed;
        baseScale = transform.localScale;

        initSound();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (levelCanvas == null || gameManager == null || powerupManager == null)
        {
            initManagers();
        }

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

    public void initSound()
    {
        powerupSound = gameObject.AddComponent<AudioSource>();
        powerupSound.clip = (AudioClip)Resources.Load("Sound/PowerupPong/UsePowerup");

        collectSound = gameObject.AddComponent<AudioSource>();
        collectSound.clip = Resources.Load<AudioClip>("Sound/PowerupPong/CollectPowerup");
    }

    public void initManagers()
    {
        // Init managers
        levelCanvas = FindObjectOfType<PowerupPongCanvas>();
        gameManager = FindObjectOfType<PowerupPongGameManager>();
        powerupManager = FindObjectOfType<PowerupPongPowerupManager>();
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
            if (!IsInvoking())
            {
                Invoke("usePowerup", 3);
            }  
        }

        if (botBall != null)
        {
            if (transform.position.x - botBall.transform.position.x < 1)
            {
                // Faster when very close to the ball
                body.velocity *= 1.5f;
            }
        }
    }

    // Custom initialization
    public override void init(bool onLeft)
    {
        // Init position
        leftPosition = new Vector3(-6, 0, 0);
        rightPosition = new Vector3(6, 0, 0);
        base.init(onLeft);

        // Init Color, onLeft
        Color color = onLeft ? leftColor : rightColor;
        GetComponent<SpriteRenderer>().color = color;
        this.onLeft = onLeft;
        InvokeClientRpcOnEveryone(UpdateVarsOnClient, color, onLeft);
    }

    public override void setupBot()
    {
        // Init position
        leftPosition = new Vector3(-6, 0, 0);
        rightPosition = new Vector3(6, 0, 0);
        base.setupBot();

        // Init Color, onLeft
        Color color = rightColor;
        GetComponent<SpriteRenderer>().color = color;
        onLeft = false;
        InvokeClientRpcOnEveryone(UpdateVarsOnClient, color, onLeft);
    }

    [ClientRPC]
    public void UpdateVarsOnClient(Color color, bool onLeft)
    {
        GetComponent<SpriteRenderer>().color = color;
        this.onLeft = onLeft;
    }

    //---------------------------------------------
    // Powerup System
    //---------------------------------------------

    /// <summary>
    /// Gives the paddle the powerup - Server-side
    /// </summary>
    /// <param name="powerup">Powerup object to give the paddle</param>
    /// <returns>False if paddle already has a powerup. True if powerup was granted.</returns>
    public bool givePowerup(PowerupPongPowerupManager.PowerupType power)
    {
        if (this.power == PowerupPongPowerupManager.PowerupType.None)
        {
            this.power = power;
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

        collectSound.Play();
        levelCanvas.setPowerupImage(power, onLeft);
    }

    // Client-Side
    private void usePowerup()
    {
        float modifier = powerupManager.getModifier(power);
        int duration = powerupManager.getDuration(power);

        switch (power)
        {
            case PowerupPongPowerupManager.PowerupType.BallSpeedDown:
                gameManager.ChangeBallSpeed(modifier, duration);
                break;
            case PowerupPongPowerupManager.PowerupType.BallSpeedUp:
                gameManager.ChangeBallSpeed(modifier, duration);
                break;
            case PowerupPongPowerupManager.PowerupType.BallReverse:
                gameManager.ReverseBall();
                break;
            case PowerupPongPowerupManager.PowerupType.PaddleSpeedDown:
                gameManager.ChangePaddleSpeed(modifier, duration, !onLeft);
                break;
            case PowerupPongPowerupManager.PowerupType.PaddleSpeedUp:
                gameManager.ChangePaddleSpeed(modifier, duration, onLeft);
                break;
            case PowerupPongPowerupManager.PowerupType.PaddleExpand:
                gameManager.ChangePaddleScale(modifier, duration, onLeft);
                break;
            case PowerupPongPowerupManager.PowerupType.PaddleShrink:
                gameManager.ChangePaddleScale(modifier, duration, !onLeft);
                break;
            case PowerupPongPowerupManager.PowerupType.WorldReverse:
                gameManager.WorldReverse();
                break;
        }

        if (isBot)
        {
            ClearPowerupOnClient(onLeft);
        }
        SetPowerupToNone();
    }

    [ServerRPC]
    public void SetPowerupToNone()
    {
        power = PowerupPongPowerupManager.PowerupType.None;
        if (IsServer)
        {
            InvokeClientRpcOnEveryone(ClearPowerupOnClient, onLeft);
        } else
        {
            InvokeServerRpc(SetPowerupToNone);
        }
    }

    [ClientRPC]
    public void ClearPowerupOnClient(bool onLeft)
    {
        powerupSound.Play();
        levelCanvas.clearPowerupImage(onLeft);
    }

    [ClientRPC]
    public void ChangePaddleSpeedOnClient(float modifier, int duration)
    {
        IEnumerator changePaddleSpeedCoroutine = ChangePaddleSpeed(modifier, duration);
        StartCoroutine(changePaddleSpeedCoroutine);
    }

    public IEnumerator ChangePaddleSpeed(float modifier, int duration)
    {
        TrailRenderer trail = GetComponent<TrailRenderer>();
        speed *= modifier;
        if (modifier > 1)
        {
            trail.emitting = true;
        }

        // If duration < 0, effect lasts forever
        if (duration >= 0)
        {
            yield return new WaitForSeconds(duration);
            speed = baseSpeed;
            trail.emitting = false;
        }
    }

    [ClientRPC]
    public void ChangePaddleScaleOnClient(float modifier, int duration)
    {
        StartCoroutine(ChangePaddleScale(modifier, duration));
    }

    public IEnumerator ChangePaddleScale(float modifier, int duration)
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * modifier, transform.localScale.z);
        // If duration < 0, effect lasts forever
        if (duration >= 0)
        {
            yield return new WaitForSeconds(duration);
            transform.localScale = baseScale;
        }
        yield break;
    }
}

