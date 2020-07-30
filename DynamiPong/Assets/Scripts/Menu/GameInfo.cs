using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    public enum WinCondition {Freeplay, FirstTo15, FirstTo30, MostAfter5, MostAfter10}

    public bool useBot = false;
    public WinCondition winCon = WinCondition.Freeplay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
