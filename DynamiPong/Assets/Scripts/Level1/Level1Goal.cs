using UnityEngine;

public class Level1Goal : GoalBehaviour
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        manager = FindObjectOfType<Level1GameManager>();

        // Color the goal based on side
        GetComponent<SpriteRenderer>().color = onLeft ? Color.green : Color.red;
    } 
}

