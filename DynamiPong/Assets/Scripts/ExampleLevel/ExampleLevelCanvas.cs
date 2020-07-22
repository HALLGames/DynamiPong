using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleLevelCanvas : LevelCanvasBehaviour
{
    // Start is called before the first frame update
    new void Start()
    {
        // Assign colors
        leftScoreText.color = Color.green;
        rightScoreText.color = Color.red;
    }
}
