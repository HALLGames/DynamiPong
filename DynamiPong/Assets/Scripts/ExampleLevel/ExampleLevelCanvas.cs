using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleLevelCanvas : LevelCanvasBehaviour
{
    // Start is called before the first frame update
    new void Start()
    {
        // Assign colors
        playerScores.leftPlayerScore.color = Color.green;
        playerScores.rightPlayerScore.color = Color.red;
    }
}
