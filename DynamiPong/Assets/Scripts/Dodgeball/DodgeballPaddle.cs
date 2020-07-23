using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;


public class DodgeballPaddle : PaddleBehaviour
{
    protected bool onLeft;

    protected GameManagerBehaviour manager;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ball")
        {
            // Hit by ball
            manager.scoreGoal(onLeft);
        }
    }
    // Start is called before the first frame update
    new void Start()
    {
        onLeft = transform.position.x < 0;
        manager = FindObjectOfType<GameManagerBehaviour>();
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    // Custom player movement
    protected override void playerMovement()
    {
        base.playerMovement();
    }

    // Custom bot movement
    protected override void botMovement()
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
            if(ballY <= 0)
            {
                body.velocity = new Vector2(0, paddleY +4);
            }
            else
            {
                body.velocity = new Vector2(0, paddleY -5);
            }

        }

        // Sync
        InvokeClientRpcOnEveryone(UpdateBotMovementOnClient, transform.position, body.velocity);
    }

    // Custom initialization
    public override void init(bool onLeft)
    {
        base.init(onLeft);
    }
}

