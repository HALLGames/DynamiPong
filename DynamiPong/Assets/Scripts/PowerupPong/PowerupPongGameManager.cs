using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

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

    [ServerRPC]
    public void ChangeBallSpeedOnServer(float modifier, int duration)
    {
        StartCoroutine(ball.GetComponent<PowerupPongBall>().ChangeSpeed(modifier, duration));
    }

    [ServerRPC]
    public void ChangeRemotePaddleSpeed(float modifier, int duration, bool fromLeft)
    {
        ulong clientId = fromLeft ? rightPaddle.OwnerClientId : leftPaddle.OwnerClientId;
        InvokeClientRpcOnClient(ChangeRemotePaddleSpeedOnClient, clientId, modifier, duration, fromLeft);
    }

    [ClientRPC]
    public void ChangeRemotePaddleSpeedOnClient(float modifier, int duration, bool fromLeft)
    {
        if (fromLeft)
        {
            StartCoroutine(rightPaddle.GetComponent<PowerupPongPaddle>().ChangePaddleSpeed(modifier, duration));
        } else
        {
            StartCoroutine(leftPaddle.GetComponent<PowerupPongPaddle>().ChangePaddleSpeed(modifier, duration));
        }
    }

    [ServerRPC]
    public void ChangeRemotePaddleScale(float modifier, int duration, bool fromLeft)
    {
        ulong clientId = fromLeft ? rightPaddle.OwnerClientId : leftPaddle.OwnerClientId;
        InvokeClientRpcOnClient(ChangeRemotePaddleScaleOnClient, clientId, modifier, duration, fromLeft);
    }

    [ClientRPC]
    public void ChangeRemotePaddleScaleOnClient(float modifier, int duration, bool fromLeft)
    {
        if (fromLeft)
        {
            StartCoroutine(rightPaddle.GetComponent<PowerupPongPaddle>().ChangePaddleScale(modifier, duration));
        }
        else
        {
            StartCoroutine(leftPaddle.GetComponent<PowerupPongPaddle>().ChangePaddleScale(modifier, duration));
        }
    }

    [ServerRPC]
    public void ReverseBallOnServer()
    {
        ball.GetComponent<PowerupPongBall>().reverse();
    }

    [ServerRPC]
    public void WorldReverse()
    {
        InvokeClientRpcOnEveryone(WorldReverseOnClient);
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


