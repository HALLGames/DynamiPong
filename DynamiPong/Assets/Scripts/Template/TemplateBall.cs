using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class TemplateBall : BallBehaviour
{
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

    protected override void launchBall()
    {
        base.launchBall();
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
    }
}

