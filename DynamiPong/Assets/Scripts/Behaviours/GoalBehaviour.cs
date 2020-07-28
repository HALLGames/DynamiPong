﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalBehaviour : MonoBehaviour
{
    protected bool onLeft;
    protected GameManagerBehaviour manager;

    protected AudioSource goalHit;

    // Does NOT get called by Unity
    // Call this method with base.Start() in the method "new void Start()"
    protected void Start()
    {
        initSound();

        manager = FindObjectOfType<GameManagerBehaviour>();
        onLeft = transform.position.x < 0;
    }

    protected virtual void initSound()
    {
        // initializes the goal hit and paddle hit sound effects
        goalHit = gameObject.AddComponent<AudioSource>();
        goalHit.clip = (AudioClip)Resources.Load("Sound/GoalHit");
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
