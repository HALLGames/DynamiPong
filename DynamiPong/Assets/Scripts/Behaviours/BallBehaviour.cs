using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class BallBehaviour : NetworkedBehaviour
{
    public float speed = 5.0f;

    protected Rigidbody2D body;

    public override void NetworkStart()
    {
        // Get body
        body = GetComponent<Rigidbody2D>();

        launchBall();
    }

    /// <summary>
    /// Does NOT get called by Unity
    /// Call this method with base.Start() in the method "new void Start()"
    /// </summary>
    protected void Start()
    {
        
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
    /// Override this method to customize the launching of the ball
    /// </summary>
    protected virtual void launchBall()
    {
        // Launch in pseudo-random direction
        body.velocity = BallBehaviour.randomNormalizedVelocity() * speed;
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

    /// <summary>
    /// Override this method to add custom collision logic
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        // Prevent sticking to the wall
        if (collision.transform.tag == "Wall")
        {
            if (body.velocity.y > 0 && body.velocity.y < 0.25f)
            {
                // Hit top wall with low velocity - launch down
                body.velocity = new Vector2(body.velocity.x, -0.5f);
            }
            else if (body.velocity.y < 0 && body.velocity.y > -0.25f)
            {
                // Hit bottom wall with low velocity - launch up
                body.velocity = new Vector2(body.velocity.x, 0.5f);
            }
        }
    }

    // Returns a pseudo-random Vector2 with a magnitude of 1
    public static Vector2 randomNormalizedVelocity()
    {
        Vector2 newVelocity;
        // Pseudo-random starting direction
        // x-dir is always either 1 or -1, y-dir is a float between 1 and -1
        if (Random.value > 0.5)
        {
            newVelocity = new Vector2(1f, Random.Range(-1f, 1f));
        }
        else
        {
            newVelocity = new Vector2(-1f, Random.Range(-1f, 1f));
        }
        return newVelocity.normalized;
    }
}
