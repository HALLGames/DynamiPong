using UnityEngine;

public class Level1Goal : GoalBehaviour
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        // Color the goal based on side
        GetComponent<SpriteRenderer>().color = onLeft ? Color.green : Color.red;
    } 
}

