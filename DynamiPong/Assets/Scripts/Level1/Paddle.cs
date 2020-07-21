using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

namespace Level1
{
    public class Paddle : NetworkedBehaviour
    {
        public float speed = 10.0f;

        private Rigidbody2D body;
        private const string InputSource = "Vertical";
        private Vector3 leftPosition = new Vector3(-7f, 0, 0);
        private Vector3 rightPosition = new Vector3(7f, 0, 0);

        // Bot stuff
        private bool isBot = false;
        private Ball botBall = null;
        private bool isBoosting;

        // Start is called before the first frame update
        void Start()
        {
            // Init
            body = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {
            if (isBot)
            {
                // Bot logic
                moveBot();
                return;
            }

            if (IsOwner)
            {
                // Local Player

                // Movement: Player input
                body.velocity = new Vector2(0.0f, Input.GetAxis(InputSource)) * speed;

                // Sync position: Client -> Server
                InvokeServerRpc(SendMovementToServer, transform.position, body.velocity);
            }

            if (IsServer)
            {
                // Remote Player
                InvokeClientRpcOnEveryone(UpdateRemotePlayerMovementOnClient, transform.position, body.velocity);
            }
        }

        void moveBot()
        {
            // Find Ball
            if (botBall == null)
            {
                botBall = GameObject.FindObjectOfType<Ball>();
            }

            // Movement
            if (botBall != null)
            {
                float ballY = botBall.transform.position.y;
                float paddleY = transform.position.y;

                // Normal movement
                body.velocity = new Vector2(0, ballY - paddleY) * 4;

                if (isBoosting)
                {
                    // Boost Movement
                    body.velocity = body.velocity * 3;
                }

                // Chance based logic
                float rand = UnityEngine.Random.value;
                if (rand < 0.01)
                {
                    // Small chance to enable boost
                    isBoosting = true;
                }
                else if (rand < 0.05)
                {
                    // Chance to end boost
                    isBoosting = false;
                }
            }

            // Sync
            InvokeClientRpcOnEveryone(UpdateBotMovementOnClient, transform.position, body.velocity);
        }

        // Called by GameManager (Server-side)
        public void setSide(bool onLeft)
        {
            // Left: Green
            // Right: Red
            transform.position = onLeft ? leftPosition : rightPosition;
            Color color = onLeft ? Color.green : Color.red;
            InvokeClientRpcOnEveryone<Vector3, Color>(SetSideOnClient, transform.position, color);
        }

        // Paddles on client get initial position and color info
        [ClientRPC]
        public void SetSideOnClient(Vector3 position, Color color)
        {
            transform.position = position;
            GetComponent<SpriteRenderer>().color = color;
        }

        // The server gets position from client
        [ServerRPC]
        public void SendMovementToServer(Vector3 position, Vector2 velocity)
        {
            transform.position = position;
            body.velocity = velocity;
        }

        // Remote paddle on client gets position from server
        [ClientRPC]
        public void UpdateRemotePlayerMovementOnClient(Vector3 position, Vector2 velocity)
        {
            if (!IsOwner)
            {
                transform.position = position;
                body.velocity = velocity;
            }
        }

        [ClientRPC]
        private void UpdateBotMovementOnClient(Vector3 position, Vector2 velocity)
        {
            transform.position = position;
            body.velocity = velocity;
        }

        public void setupBot()
        {
            isBot = true;
            transform.position = rightPosition;
        }
    }
}
