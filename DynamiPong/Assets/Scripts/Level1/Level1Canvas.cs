using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Canvas : LevelCanvasBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Assign colors
        leftScoreText.color = Color.green;
        rightScoreText.color = Color.red;
    }
}
