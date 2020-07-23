using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class PaddleBehaviour : NetworkedBehaviour
{
    public float speed = 10.0f;

    protected Rigidbody2D body;
    protected const string InputSource = "Vertical";
    protected Vector3 leftPosition = new Vector3(-7f, 0, 0);
    protected Vector3 rightPosition = new Vector3(7f, 0, 0);

    // Bot
    protected bool isBot = false;
    protected BallBehaviour botBall = null;

    /// <summary>
    /// Does NOT get called by Unity
    /// Call this method with base.Start() in the method "new void Start()"
    /// </summary>
    protected void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Does NOT get called by Unity
    /// Call this method with base.Update() in the method "new void Update()"
    /// </summary>
    protected void Update()
    {
        if (isBot)
        {
            // Bot logic
            botMovement();
            return;
        }

        if (IsOwner)
        {
            // Local Player
            playerMovement();

            // Sync position: Client -> Server
            InvokeServerRpc(SendMovementToServer, transform.position, body.velocity);
        }

        if (IsServer)
        {
            // Remote Player
            InvokeClientRpcOnEveryone(UpdateRemotePlayerMovementOnClient, transform.position, body.velocity);
        }
    }

    /// <summary>
    /// Override this method to add custom paddle movement (client-side)
    /// </summary>
    protected virtual void playerMovement()
    {
        // Movement: Player input
        body.velocity = new Vector2(0.0f, Input.GetAxis(InputSource)) * speed;
    }

    /// <summary>
    /// Override this method to add custom bot logic
    /// </summary>
    protected virtual void botMovement()
    {
        // Find Ball
        if (botBall == null)
        {
            botBall = GameObject.FindObjectOfType<BallBehaviour>();
        }

        // Movement
        if (botBall != null)
        {
            float ballY = botBall.transform.position.y;
            float paddleY = transform.position.y;

            // Normal movement
            body.velocity = new Vector2(0, ballY - paddleY) * speed;
        }

        // Sync
        InvokeClientRpcOnEveryone(UpdateBotMovementOnClient, transform.position, body.velocity);
    }

    //--------------------------------------------------
    // Setup
    //--------------------------------------------------

    /// <summary>
    /// Places the paddle in the left position if onLeft is true and right otherwise.
    /// Override this method to add more behaviour when the paddles are spawned.
    /// </summary>
    /// <param name="onLeft"></param>
    public virtual void init(bool onLeft)
    {
        transform.position = onLeft ? leftPosition : rightPosition;
        InvokeClientRpcOnEveryone(SetSideOnClient, transform.position);
    }

    // Paddles on client get initial position and color info
    [ClientRPC]
    public void SetSideOnClient(Vector3 position)
    {
        transform.position = position;
    }

    /// <summary>
    /// Override this method to add more behaviour when a bot is spawned.
    /// </summary>
    public virtual void setupBot()
    {
        isBot = true;
        transform.position = rightPosition;
        speed = 4.0f;
    }

    //--------------------------------------------------
    // Movement RPCs
    //--------------------------------------------------

    // The server gets position from client
    [ServerRPC]
    public void SendMovementToServer(Vector3 position, Vector2 velocity)
    {
        transform.position = position;
        body.velocity = velocity;
    }

    // Remote paddle on client gets position from server
    [ClientRPC]
    public void UpdateRemotePlayerMovementOnClient(Vector3 position, Vector2 velocity)
    {
        if (!IsOwner)
        {
            transform.position = position;
            body.velocity = velocity;
        }
    }

    // Client gets info about bot position
    [ClientRPC]
    public void UpdateBotMovementOnClient(Vector3 position, Vector2 velocity)
    {
        transform.position = position;
        body.velocity = velocity;
    }
}
