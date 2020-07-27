using UnityEngine;

public class PowerupPongGoal : GoalBehaviour
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        // Color based on side
        GetComponent<SpriteRenderer>().color = onLeft ? Color.green : Color.red;
    } 
}

