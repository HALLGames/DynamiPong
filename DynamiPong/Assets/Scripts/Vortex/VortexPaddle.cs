﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;


public class VortexPaddle : PaddleBehaviour
{
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

    // Custom player movement
    protected override void playerMovement()
    {
        base.playerMovement();
    }

    // Custom bot movement
    protected override void botMovement()
    {
        base.botMovement();
        body.velocity = body.velocity * 3;
    }

    // Custom initialization
    public override void init(bool onLeft)
    {
        base.init(onLeft);
    }
}

