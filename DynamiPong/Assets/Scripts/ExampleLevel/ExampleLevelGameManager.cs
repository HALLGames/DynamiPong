using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using UnityEngine.UI;
using MLAPI.Messaging;
using MLAPI.SceneManagement;
using UnityEngine.SceneManagement;

public class ExampleLevelGameManager : GameManagerBehaviour
{
    public override void NetworkStart()
    {
        base.NetworkStart();
    }

    protected override void getPrefabs()
    {
        ballPrefab = Network.GetPrefab<ExampleLevelBall>("ExampleLevelBall");
        paddlePrefab = Network.GetPrefab<ExampleLevelPaddle>("ExampleLevelPaddle");
    }
}


