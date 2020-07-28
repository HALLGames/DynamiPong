using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine.UI;

public class PowerupPongGameManager : GameManagerBehaviour
{
    public override void NetworkStart()
    {
        base.NetworkStart();
    }

    protected override void getPrefabs()
    {
        ballPrefab = Network.GetPrefab<PowerupPongBall>("PowerupPongBall");
        paddlePrefab = Network.GetPrefab<PowerupPongPaddle>("PowerupPongPaddle");
    }

    public override void respawnBall()
    {
        ball.StopAllCoroutines();
        base.respawnBall();
    }


    [ServerRPC(RequireOwnership = false)]
    public void ChangeBallSpeed(float modifier, int duration)
    {
        if (IsServer)
        {
            ball.StopAllCoroutines();
            ball.StartCoroutine(ball.GetComponent<PowerupPongBall>().ChangeSpeed(modifier, duration));
        } 
        else
        {
            InvokeServerRpc(ChangeBallSpeed, modifier, duration);
        }
    }

    [ServerRPC(RequireOwnership = false)]
    public void ChangePaddleSpeed(float modifier, int duration, bool onLeft)
    {
        if (IsServer)
        {
            PowerupPongPaddle paddle = onLeft ? leftPaddle.GetComponent<PowerupPongPaddle>() : rightPaddle.GetComponent<PowerupPongPaddle>();
            if (paddle.IsBot())
            {
                paddle.ChangePaddleSpeedOnClient(modifier, duration);
            }
            paddle.InvokeClientRpcOnEveryone(paddle.ChangePaddleSpeedOnClient, modifier, duration);
        }
        else
        {
            InvokeServerRpc(ChangePaddleSpeed, modifier, duration, onLeft);
        }
    }

    [ServerRPC(RequireOwnership = false)]
    public void ChangePaddleScale(float modifier, int duration, bool onLeft)
    {
        if (IsServer)
        {
            PowerupPongPaddle paddle = onLeft ? leftPaddle.GetComponent<PowerupPongPaddle>() : rightPaddle.GetComponent<PowerupPongPaddle>();
            if (paddle.IsBot())
            {
                paddle.ChangePaddleScaleOnClient(modifier, duration);
            }
            paddle.InvokeClientRpcOnEveryone(paddle.ChangePaddleScaleOnClient, modifier, duration);
        }
        else
        {
            InvokeServerRpc(ChangePaddleScale, modifier, duration, onLeft);
        }
    }

    [ServerRPC(RequireOwnership = false)]
    public void ReverseBall()
    {
        if (IsServer)
        {
            ball.GetComponent<PowerupPongBall>().reverse();
        }
        else
        {
            InvokeServerRpc(ReverseBall);
        }
    }

    [ServerRPC(RequireOwnership = false)]
    public void WorldReverse()
    {
        if (IsServer)
        {
            InvokeClientRpcOnEveryone(WorldReverseOnClient);
        }
        else
        {
            InvokeServerRpc(WorldReverse);
        }
    }

    [ClientRPC]
    public void WorldReverseOnClient()
    {
        StartCoroutine(RotateCameraOverSeconds(90, 2));
    }

    private IEnumerator RotateCameraOverSeconds(float degreesToRotate, float duration)
    {
        Camera camera = FindObjectOfType<Camera>();

        float rotatedDegrees = 0;
        float degreesPerSecond = degreesToRotate / duration;
        while (rotatedDegrees < degreesToRotate)
        {
            camera.transform.Rotate(0, 0, degreesPerSecond * Time.deltaTime);
            rotatedDegrees += degreesPerSecond * Time.deltaTime;
            yield return null;
        }

        // Snap to nearest 180 deg rotation
        if (Mathf.Pow(camera.transform.rotation.eulerAngles.z - 180, 2) < Mathf.Pow(camera.transform.rotation.eulerAngles.z - 0, 2))
        {
            camera.transform.eulerAngles = new Vector3(0, 0, 180);
        } else
        {
            camera.transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }
}


