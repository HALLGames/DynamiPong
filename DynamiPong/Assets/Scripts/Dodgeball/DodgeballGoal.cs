using UnityEngine;

public class DodgeballGoal : GoalBehaviour
{
    protected DodgeballBall dodgeballBall;

    // Does NOT get called by Unity
    // Call this method with base.Start() in the method "new void Start()"
    protected new void Start()
    {
        dodgeballBall = FindObjectOfType<DodgeballBall>();
    }

    // Override this method for custom the goal trigger
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ball")
        {
            // Hit by ball
            dodgeballBall.increaseSpeed();
        }
    }
}

