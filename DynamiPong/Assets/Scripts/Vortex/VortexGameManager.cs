using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class VortexGameManager : GameManagerBehaviour
{
    public override void NetworkStart()
    {
        base.NetworkStart();
    }

    protected override void getPrefabs()
    {
        ballPrefab = Network.GetPrefab<TemplateBall>("TemplateBall");
        paddlePrefab = Network.GetPrefab<TemplatePaddle>("TemplatePaddle");
    }
}


