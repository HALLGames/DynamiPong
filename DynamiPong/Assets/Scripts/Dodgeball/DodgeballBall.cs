using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class DodgeballBall : BallBehaviour
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        // Launch in pseudo-random direction
        body.velocity = randomNormalizedVelocity() * speed;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    // Custom ball movement
    protected override void ballMovement()
    {
        // Clamp x-velocity a little, in order to stop the ball from getting caught in the middle
        float normX = body.velocity.normalized.x;
        if (normX > 0.1)
        {
            normX = Mathf.Clamp(normX, 0.5f, 1f);
        }
        else
        {
            normX = Mathf.Clamp(normX, -1f, -0.5f);
        }

        // Clamp y-velocity a little, in order to stop the ball from getting caught in the middle
        float normY = body.velocity.normalized.y;
        if (normY > 0.1)
        {
            normY = Mathf.Clamp(normY, 0.5f, 1f);
        }
        else
        {
            normY = Mathf.Clamp(normY, -1f, -0.5f);
        }

        // Keep velocity constant
        body.velocity = new Vector2(normX, normY) * speed;
    }

    // Custom collision logic
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (collision.transform.tag == "Goal")
        {
            manager.PlaySound("Wall");
        }
    }

    public void increaseSpeed()
    {
        speed += 1;
    }
}

