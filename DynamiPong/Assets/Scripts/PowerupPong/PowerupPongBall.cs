using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class PowerupPongBall : BallBehaviour
{
    protected SpriteRenderer spriteRenderer;
    protected TrailRenderer trail;
    private float baseSpeed;

    [HideInInspector]
    public PowerupPongPaddle lastTouchedPaddle;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();
        trail = GetComponent<TrailRenderer>();

        spriteRenderer.color = Color.white;
        lastTouchedPaddle = null;
        baseSpeed = speed;
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
                spriteRenderer.color = collision.transform.GetComponent<SpriteRenderer>().color;
                lastTouchedPaddle = collision.transform.GetComponent<PowerupPongPaddle>();

                InvokeClientRpcOnEveryone(UpdateColorOnClients, spriteRenderer.color);
            }
        }
    }

    [ClientRPC]
    public void UpdateColorOnClients(Color color)
    {
        spriteRenderer.color = color;
    }

    public IEnumerator ChangeSpeed(float modifier, int duration)
    {
        speed *= modifier;
        if (modifier > 1)
        {
            InvokeClientRpcOnEveryone(ToggleTrailOnClient, true);
        }
        // If duration < 0, effect lasts forever
        if (duration >= 0)
        {
            yield return new WaitForSeconds(duration);
            speed = baseSpeed;
            InvokeClientRpcOnEveryone(ToggleTrailOnClient, false);
        }
    }

    [ClientRPC]
    public void ToggleTrailOnClient(bool emitting)
    {
        trail.emitting = emitting;
    }

    public void reverse()
    {
        body.velocity = -body.velocity;
    }
}

