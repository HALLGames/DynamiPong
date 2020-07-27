using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBall : MonoBehaviour
{
    public float speed = 5;
    private Rigidbody2D body;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();

        body.velocity = BallBehaviour.randomNormalizedVelocity() * speed;
    }

    // Update is called once per frame
    void Update()
    {
        body.velocity = body.velocity.normalized * speed;
    }

    private void OnMouseDown()
    {
        body.velocity = BallBehaviour.randomNormalizedVelocity() * speed;
    }
}
