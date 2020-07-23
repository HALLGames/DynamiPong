using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class PowerupPongBall : BallBehaviour
{
    protected new SpriteRenderer renderer;

    [HideInInspector]
    public PowerupPongPaddle lastTouchedPaddle;

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

        // Clamp x-velocity a little, in order to stop the ball from getting caught in the middle
        float normX = body.velocity.normalized.x;
        if (normX > 0)
        {
            normX = Mathf.Clamp(normX, 0.5f, 1f);
        }
        else
        {
            normX = Mathf.Clamp(normX, -1f, -0.5f);
        }

        // Keep velocity constant
        body.velocity = new Vector2(normX, body.velocity.normalized.y) * speed;
    }

    // Custom collision logic
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (IsServer)
        {
            // Collision with Paddle
            if (collision.transform.tag == "Paddle")
            {
                // Transform into the color of the last touched paddle
                renderer.color = collision.transform.GetComponent<SpriteRenderer>().color;
                lastTouchedPaddle = collision.transform.GetComponent<PowerupPongPaddle>();

                if (IsServer)
                {
                    InvokeClientRpcOnEveryone(UpdateColorOnClients, renderer.color);
                }
            }
        }
    }

    [ClientRPC]
    public void UpdateColorOnClients(Color color)
    {
        renderer.color = color;
    }

    public IEnumerator ChangeSpeed(float speedModifier, int duration)
    {
        float oldSpeed = speed;
        speed *= speedModifier;
        // If duration < 0, effect lasts forever
        if (duration >= 0)
        {
            yield return new WaitForSeconds(duration);
            speed = oldSpeed;
        }
    }

    public void reverse()
    {
        body.velocity = -body.velocity;
    }
}

