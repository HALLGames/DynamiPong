using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class BallBehaviour : NetworkedBehaviour
{
    public float speed = 5.0f;

    protected Rigidbody2D body;

    protected AudioSource wallHit;
    protected AudioSource paddleHit;

    public override void NetworkStart()
    {
        // Get body
        body = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Does NOT get called by Unity
    /// Call this method with base.Start() in the method "new void Start()"
    /// </summary>
    protected void Start()
    {
        // Get body
        body = GetComponent<Rigidbody2D>();

        initSound();

        launchBall();
    }

    /// <summary>
    /// Override this method to customize what sounds are played
    /// </summary>
    protected virtual void initSound()
    {
        // initializes the wall hit and paddle hit sound effects
        wallHit = gameObject.AddComponent<AudioSource>();
        wallHit.clip = Resources.Load<AudioClip>("Sound/Common/WallHit");

        paddleHit = gameObject.AddComponent<AudioSource>();
        paddleHit.clip = Resources.Load<AudioClip>("Sound/Common/PaddleHit");
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

        if (collision.transform.tag == "Paddle")
        {
            if (paddleHit.enabled)
            {
                paddleHit.Play();
            }
        }

        if (collision.transform.tag == "Wall")
        {
            wallHit.Play();

            // Prevent sticking to the wall
            if (transform.position.y >= 0 && Mathf.Abs(body.velocity.y) <= 1.5f)
            {
                // Hit top wall with low velocity - launch down
                body.velocity = new Vector2(body.velocity.x, -1.5f);
            }
            else if (transform.position.y <= 0 && Mathf.Abs(body.velocity.y) <= 1.5f)
            {
                // Hit bottom wall with low velocity - launch up
                body.velocity = new Vector2(body.velocity.x, 1.5f);
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
