﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;


public class VortexBall : BallBehaviour
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
        float normY = body.velocity.normalized.y;
        if (body.position.x > 2f || body.position.x < -2f)
        {
            if (normX > 0.1)
            {
                normX = Mathf.Clamp(normX, 0.5f, 1f);
            }
            else
            {
                normX = Mathf.Clamp(normX, -1f, -0.5f);
            }
        }
        

        // Keep velocity constant
        body.velocity = new Vector2(normX, normY) * speed;
    }
}

