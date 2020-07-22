using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class PowerupPongBall : BallBehaviour
{
    protected new SpriteRenderer renderer;
    protected PowerupPongPaddle lastTouchedPaddle;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        renderer = GetComponent<SpriteRenderer>();
        renderer.color = Color.white;
        lastTouchedPaddle = null;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    // Custom ball movement
    protected override void ballMovement()
    {
        base.ballMovement();
    }

    // Custom collision logic
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        // Transform into the color of the last touched paddle
        if (collision.transform.tag == "Paddle")
        {
            renderer.color = collision.transform.GetComponent<SpriteRenderer>().color;
            lastTouchedPaddle = collision.transform.GetComponent<PowerupPongPaddle>();

            if (IsServer)
            {
                InvokeClientRpcOnEveryone(UpdateColorOnClients, renderer.color);
            }
        }
    }

    [ClientRPC]
    public void UpdateColorOnClients(Color color)
    {
        renderer.color = color;
    }
}

