using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class BallBehaviour : NetworkedBehaviour
{
    public float speed = 5.0f;

    protected Rigidbody2D body;

    /// <summary>
    /// Does NOT get called by Unity
    /// Call this method with base.Start() in the method "new void Start()"
    /// </summary>
    protected void Start()
    {
        // Get body
        body = GetComponent<Rigidbody2D>();

        // Move
        body.velocity = Vector2.one.normalized * speed;
    }

    /// <summary>
    /// Does NOT get called by Unity
    /// Call this method with base.Update() in the method "new void Update()"
    /// </summary>
    protected void Update()
    {
        ballMovement();

        // Sync position
        if (IsServer)
        {
            InvokeClientRpcOnEveryone(SendMovementToClients, transform.position, body.velocity);
        }
    }

    /// <summary>
    /// Override this method to add custom movement logic to the ball
    /// </summary>
    protected virtual void ballMovement()
    {

    }

    [ClientRPC]
    public void SendMovementToClients(Vector3 position, Vector2 velocity)
    {
        transform.position = position;
        body.velocity = velocity;
    }
}
