﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class Level2GameManager : GameManagerBehaviour
{
    public override void NetworkStart()
    {
        base.NetworkStart();
    }

    protected override void getPrefabs()
    {
        ballPrefab = Network.GetPrefab<PowerupPongBall>("TemplateBall");
        paddlePrefab = Network.GetPrefab<PowerupPongPaddle>("TemplatePaddle");
    }
}
