using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalBehaviour : MonoBehaviour
{
    protected bool onLeft;
    protected GameManagerBehaviour manager;

    

    // Does NOT get called by Unity
    // Call this method with base.Start() in the method "new void Start()"
    protected void Start()
    {
        manager = FindObjectOfType<GameManagerBehaviour>();
        onLeft = transform.position.x < 0;
    }

    // Override this method for custom the goal trigger
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ball")
        {
            manager.PlaySound("Goal");

            // Hit by ball
            manager.scoreGoal(onLeft);
        }
    }
}
