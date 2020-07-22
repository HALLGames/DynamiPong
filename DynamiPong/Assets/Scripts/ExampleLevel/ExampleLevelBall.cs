using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;


public class ExampleLevelBall : BallBehaviour
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

    // Returns a pseudo-random Vector2 with a magnitude of 1
    private Vector2 randomNormalizedVelocity()
    {
        Vector2 newVelocity;
        // Pseudo-random starting direction
        // x-dir is always either 1 or -1, y-dir is a float between 1 and -1
        if (Random.value > 0.5)
        {
            newVelocity = new Vector2(1f, Random.Range(-1f, 1f));
        }
        else
        {
            newVelocity = new Vector2(-1f, Random.Range(-1f, 1f));
        }
        return newVelocity.normalized;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }
}

