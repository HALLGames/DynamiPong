using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class DodgeballGameManager : GameManagerBehaviour
{
    public override void NetworkStart()
    {
        base.NetworkStart();
    }

    protected override void getPrefabs()
    {
        ballPrefab = Network.GetPrefab<DodgeballBall>("DodgeballBall");
        paddlePrefab = Network.GetPrefab<DodgeballPaddle>("DodgeballPaddle");
    }
}


