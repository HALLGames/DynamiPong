using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

namespace Level1
{
    public class Ball : NetworkedBehaviour
    {
        public float speed = 5.0f;

        private Rigidbody2D body;

        // Start is called before the first frame update
        void Start()
        {
            // Get body
            body = GetComponent<Rigidbody2D>();

            body.velocity = randomNormalizedVelocity() * speed;
        }

        // Update is called once per frame
        void Update()
        {
            // Sync position
            if (IsServer)
            {
                InvokeClientRpcOnEveryone(SendMovementToClients, transform.position, body.velocity);
            }

            // Clamp x-velocity a little, in order to stop the ball from getting caught in the middle
            float normX = body.velocity.normalized.x;
            if (normX > 0)
            {
                normX = Mathf.Clamp(normX, 0.5f, 1f);
            }
            else
            {
                normX = Mathf.Clamp(normX, -1f, -0.5f);
            }

            // Keep velocity constant
            body.velocity = new Vector2(normX, body.velocity.normalized.y) * speed;
        }

        [ClientRPC]
        public void SendMovementToClients(Vector3 position, Vector2 velocity)
        {
            transform.position = position;
            body.velocity = velocity;
        }

        // Returns a pseudo-random Vector2 with a magnitude of 1
        private Vector2 randomNormalizedVelocity()
        {
            Vector2 newVelocity;
            // Pseudo-random starting direction
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Prevent sticking to the wall
            if (collision.transform.tag == "Wall")
            {
                if (Mathf.Abs(body.velocity.y) < 0.2)
                {
                    body.velocity = new Vector2(body.velocity.x, 0.5f);
                }
            }
        }
    }
}
