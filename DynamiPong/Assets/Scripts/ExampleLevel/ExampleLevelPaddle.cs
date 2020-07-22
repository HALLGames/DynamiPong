using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class ExampleLevelPaddle : PaddleBehaviour
{
    // Bot stuff
    private bool isBoosting;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    // Custom bot movement
    protected override void botMovement()
    {
        base.botMovement();

        // Extra Movement
        if (botBall != null)
        {
            if (isBoosting)
            {
                // Boost Movement
                body.velocity = body.velocity * 3;
            }

            // Chance based logic
            float rand = UnityEngine.Random.value;
            if (rand < 0.01)
            {
                // Small chance to enable boost
                isBoosting = true;
            }
            else if (rand < 0.05)
            {
                // Chance to end boost
                isBoosting = false;
            }
        }

        // Sync
        InvokeClientRpcOnEveryone(UpdateBotMovementOnClient, transform.position, body.velocity);
    }

    // Add color to init
    public override void init(bool onLeft)
    {
        base.init(onLeft);

        // Left: Green
        // Right: Red
        Color color = onLeft ? Color.green : Color.red;
        InvokeClientRpcOnEveryone(SetColorOnClient, color);
    }

    // Paddles on client get initial position and color info
    [ClientRPC]
    public void SetColorOnClient(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }
}

