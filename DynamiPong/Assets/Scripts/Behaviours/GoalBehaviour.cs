using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalBehaviour : MonoBehaviour
{
    protected bool onLeft;
    protected GameManagerBehaviour manager;

    public AudioSource goalHit;

    // Does NOT get called by Unity
    // Call this method with base.Start() in the method "new void Start()"
    protected void Start()
    {
        goalHit = gameObject.AddComponent<AudioSource>();
        AudioClip goalClip;
        goalClip = (AudioClip)Resources.Load("Sound/GoalHit");
        goalHit.clip = goalClip;

        manager = FindObjectOfType<GameManagerBehaviour>();
        onLeft = transform.position.x < 0;
    }

    // Override this method for custom the goal trigger
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ball")
        {
            //plays goal hit sound
            goalHit.Play();
            // Hit by ball
            manager.scoreGoal(onLeft);
        }
    }
}
