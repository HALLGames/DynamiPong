using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using UnityEngine.UI;
using MLAPI.Messaging;
using MLAPI.SceneManagement;
using UnityEngine.SceneManagement;

public class Level1GameManager : GameManagerBehaviour
{
    public override void NetworkStart()
    {
        base.NetworkStart();
    }

    protected override void getPrefabs()
    {
        ballPrefab = Network.GetPrefab<Level1Ball>("Level1Ball");
        paddlePrefab = Network.GetPrefab<Level1Paddle>("Level1Paddle");
    }



}


